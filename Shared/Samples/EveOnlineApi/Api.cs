using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Collections;
using Cauldron.Core.Extensions;
using Cauldron.Cryptography;
using EveOnlineApi.Models;
using EveOnlineApi.WebService;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace EveOnlineApi
{
    [Component(typeof(IEveApi), FactoryCreationPolicy.Singleton)]
    public sealed class Api : Singleton<IEveApi>, IEveApi
    {
        private List<TokenKey> tokenInfos = new List<TokenKey>();

        [ComponentConstructor]
        public Api()
        {
        }

        public EveServers Server { get; private set; } = EveServers.Tranquility;

        public IEveStaticData StaticData { get; private set; } = new EveStaticData();

        public async Task<Motd> GetMotdAsync() => await WebServiceConsumer.Consume<DefaultRestClient, JsonDeserializer, MotdCachingAgent, Motd>(this.Server.Crest);

        public async Task InitializeAsync(EveServers server, string clientId, string secretKey)
        {
            this.Server = server;
            await this.StaticData.UpdateStaticDataAsync();
            await this.LoadAccessTokensAsync(clientId, secretKey);
            await this.CachePriceAsync();
        }

        #region CREST SSO

        private const string TokenFilename = "eve-online-api.tokens";

        /// <summary>
        /// Tries to authenticate the application using OAuth2 to CREST. The application requires a callback.
        /// Check http://eveonline-third-party-documentation.readthedocs.io/en/latest/sso/nonbrowserapps.html for more information
        /// </summary>
        /// <param name="crest">The information required for the authentification</param>
        /// <exception cref="ArgumentNullException"><paramref name="crest"/> is null</exception>
        /// <exception cref="ArgumentException">No access scope defined</exception>
        public async Task AuthorizeApplicationForCrestAsync(CrestApiKey crest, params string[] accessScopes)
        {
            if (crest == null)
                throw new ArgumentNullException(nameof(crest));

            if (accessScopes.Length == 0)
                throw new ArgumentException("At least one access scope must be defined");

            var url = $"{this.Server.Login}oauth/authorize/?response_type=code&redirect_uri={WebUtility.HtmlEncode(crest.CallbackUrl)}&client_id={crest.ClientId}&scope={WebUtility.HtmlEncode(accessScopes.Join(" "))}&state={crest.KeyId}";
            var result = await WebAuthenticationBrokerWrapper.AuthenticateAsync(new Uri(url), new Uri(crest.CallbackUrl));
            var parameters = new Uri(result).ParseQueryString();

            var basicAuth = $"{crest.ClientId}:{crest.SecretKey}".ToBase64String();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            client.DefaultRequestHeaders.Host = "login.eveonline.com";

            var response = await client.PostAsync(
                $"{this.Server.Login}oauth/token/",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"grant_type","authorization_code" },
                    {"code",parameters["code"] }
                }));

            var tokenInfo = JsonConvert.DeserializeObject<CrestTokenInfo>(await response.Content.ReadAsStringAsync());
            var characterInfo = await WebServiceConsumer.Consume<DefaultRestClient, JsonDeserializer, CrestCharacterInfo>($"{this.Server.Login}oauth/verify", tokenInfo.AccessToken, "login.eveonline.com");

            tokenInfo.Authentification = basicAuth;

            if (this.tokenInfos.Any(x => x.CharacterId == characterInfo.CharacterId))
                this.tokenInfos.RemoveAll(x => x.CharacterId == characterInfo.CharacterId);

            this.tokenInfos.Add(new TokenKey
            {
                AccessScope = accessScopes,
                CharacterId = characterInfo.CharacterId,
                TokenInfo = tokenInfo,
                ServerName = this.Server.ServerName
            });
        }

        public bool HasToken(long characterId, string accessScope) =>
            this.tokenInfos.Any(x => x.CharacterId == characterId && x.ServerName == this.Server.ServerName && x.AccessScope.Contains(accessScope));

        public async Task LoadAccessTokensAsync(string clientId, string secretKey)
        {
            if (!await ApplicationData.Current.LocalFolder.ExistsAsync(TokenFilename))
                return;
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(TokenFilename);
                var content = await file.ReadBytesAsync();

                if (content.Length < 16)
                    return;

                var passphrase = (SystemInfo.HardwareIdentifier + clientId + secretKey).ToSecureString();
                var json = Encoding.UTF8.GetString(Aes.Decrypt(passphrase, 10000, content));

                this.tokenInfos = JsonConvert.DeserializeObject<List<TokenKey>>(json);
            }
            catch (Exception)
            {
                // no need... this just means something is wrong with the cache and should be thrown away
            }
        }

        public async Task SaveAccessTokens(string clientId, string secretKey)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(TokenFilename, CreationCollisionOption.ReplaceExisting);
            var passphrase = (SystemInfo.HardwareIdentifier + clientId + secretKey).ToSecureString();

            using (var keyMaterial = KeyMaterial.CreateKeyMaterial(passphrase, 10000))
            {
                var content = Aes.Encrypt(keyMaterial, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this.tokenInfos)));
                await file.WriteBytesAsync(content);
            }
        }

        private async Task<CrestTokenInfo> GetAccessTokenAsync(long characterId, string accessScopes)
        {
            var tokenKey = this.tokenInfos.FirstOrDefault(x => x.CharacterId == characterId && x.ServerName == this.Server.ServerName && x.AccessScope.Contains(accessScopes));

            if (tokenKey == null)
                throw new UnauthorizedAccessException("You are required to authenticate using OAuth2 to be able to use this Api. For more information see http://eveonline-third-party-documentation.readthedocs.io/en/latest/sso/index.html");

            if (tokenKey.TokenInfo.ExpirationDate < DateTime.Now)
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", tokenKey.TokenInfo.Authentification);
                client.DefaultRequestHeaders.Host = "login.eveonline.com";

                var response = await client.PostAsync(
                    $"{this.Server.Login}oauth/token/",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                    {"grant_type","refresh_token" },
                    {"refresh_token",tokenKey.TokenInfo.RefreshToken }
                    }));

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<CrestTokenInfo>(data);

                tokenKey.TokenInfo = token;
            }

            return tokenKey.TokenInfo;
        }

        private async Task PostAsync(string uri, object body, CrestTokenInfo token) =>
          await this.PostAsync(uri, JsonConvert.SerializeObject(body), token);

        private async Task PostAsync(string uri, string body, CrestTokenInfo token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            client.DefaultRequestHeaders.Host = "crest-tq.eveonline.com";
            var response = await client.PostAsync(uri, new StringContent(body, Encoding.UTF8, "application/vnd.ccp.eve.Api-v3+json"));
            response.EnsureSuccessStatusCode();
        }

        #endregion CREST SSO

        #region Image Stuff

        private static ConcurrentDictionary<string, byte[]> imageCache = new ConcurrentDictionary<string, byte[]>();

        public async Task<byte[]> GetImageAsync(ImageType type, long id, double daysToCache = 1, int imageSize = 256)
        {
            var filename = string.Format("{0}_{1}_{2}.png", type, id, imageSize);

            if (imageCache.ContainsKey(filename))
                return imageCache[filename];

            var folder = ApplicationData.Current.TemporaryFolder;
            var serverStatus = await EveUtils.GetServerStatusAsync();
            byte[] data = null;

            if (await folder.ExistsAsync(filename))
            {
                var file = await folder.GetFileAsync(filename);
                if ((await file.GetDateModifiedAsync()).AddDays(daysToCache) < DateTime.Now && Network.HasInternetConnection && serverStatus == EveServerStatus.Online)
                    await file.DeleteAsync();
                else
                    data = await (await folder.GetFileAsync(filename)).ReadBytesAsync();
            }

            if (data == null)
            {
                if (!await folder.ExistsAsync(filename) && Network.HasInternetConnection && serverStatus == EveServerStatus.Online)
                {
                    string url = string.Empty;

                    switch (type)
                    {
                        case ImageType.Alliance:
                            url = this.Server.ImageAllianceUrl;
                            break;

                        case ImageType.Character:
                            url = this.Server.ImageCharacterUrl;
                            break;

                        case ImageType.Corporation:
                            url = this.Server.ImageCorpUrl;
                            break;

                        case ImageType.Item:
                            url = this.Server.ImageItemUrl;
                            break;
                    }

                    await GetAndSaveImageAsync(string.Format(url, id, imageSize), filename);
                    data = await (await folder.GetFileAsync(filename)).ReadBytesAsync();
                }
                else if (!await folder.ExistsAsync(filename))
                    return null;
            }

            if (data != null)
                imageCache.TryAdd(filename, data);

            return data;
        }

        private async Task GetAndSaveImageAsync(string url, string filename)
        {
            try
            {
                var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                await Web.DownloadFile(new Uri(url), file);
            }
            catch (Exception e)
            {
                // We actually don't care for exceptions raised during download and writing images to disk
                // Because this is just cache... Faulty images and broken downloads will are compensated

                Debug.WriteLine("GetAndSaveImageAsync: " + e.Message);
            }
        }

        #endregion Image Stuff

        #region Conquerable Stations

        public async Task<ConquerableStation> GetConquerableStationAsync(long id)
        {
            var list = await GetConquerableStationAsync();
            return list.FirstOrDefault(x => x.Id == id);
        }

        public async Task<IEnumerable<ConquerableStation>> GetConquerableStationAsync()
        {
            var result = await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, EveXmlSpecializedCollectionCacheAgent<ConquerableStation>, ConquerableStationCollection>($"{this.Server.XMLApi}eve/ConquerableStationList.xml.aspx");
            return result.Items;
        }

        public async Task<string> GetConquerableStationNameAsync(long id)
        {
            var list = await GetConquerableStationAsync();
            var station = list.FirstOrDefault(x => x.Id == id);

            if (station == null)
                return id.ToString();

            return station.Name;
        }

        #endregion Conquerable Stations

        #region Market Quicklook

        public async Task<QuicklookCollection> GetMarketOrdersAsync(long itemTypeId)
        {
            var result = await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, DefaultDiskCacheAgent, QuicklookCollection>(string.Format("http://api.eve-central.com/api/quicklook?typeid={0}", itemTypeId));
            var average = result.SellOrders.Count > 0 ? result.SellOrders.Average(x => x.Price) : 0;

            if (_itemprices.ContainsKey(itemTypeId))
                _itemprices[itemTypeId].AveragePrice = average;
            else
                _itemprices.TryAdd(itemTypeId, new ItemPrice
                {
                    ItemId = new ItemId { Id = itemTypeId },
                    AveragePrice = average,
                    AdjustedPrice = result.SellOrders.Count > 0 ? result.SellOrders.Min(x => x.Price) : 0
                });

            return result;
        }

        #endregion Market Quicklook

        #region Market Price History

        public async Task<IEnumerable<PriceHistory>> GetMarketPriceHistoryAsync(long itemTypeId, long regionId)
        {
            var result = await WebServiceConsumer.Consume<DefaultRestClient, JsonDeserializer, OneDayLifetimeDiskCacheAgent, PriceHistoryCollection>(
                $"{this.Server.Crest}market/{regionId}/types/{itemTypeId}/history/"); // TODO

            if (result == null)
                return new List<PriceHistory>();

            var average = result.Items.Average(x => x.AveragePrice);

            if (_itemprices.ContainsKey(itemTypeId))
                _itemprices[itemTypeId].AveragePrice = average;
            else
                _itemprices.TryAdd(itemTypeId, new ItemPrice
                {
                    ItemId = new ItemId { Id = itemTypeId },
                    AveragePrice = average,
                    AdjustedPrice = result.Items.Min(x => x.LowPrice)
                });

            return result.Items;
        }

        #endregion Market Price History

        #region Item Attributes

        public async Task<T> GetItemAttributesAsync<T>(long itemTypeId) where T : ItemAttributes, IKeyedModel, new() =>
            await WebServiceConsumer.Consume<DefaultRestClient, JsonDeserializer, EveXmlSpecializedCacheAgent<T>, T>(
                  $"{(await this.GetMotdAsync()).InventoryTypes.HRef}{itemTypeId}/");

        #endregion Item Attributes

        #region Item Prices

        private static ConcurrentDictionary<long, ItemPrice> _itemprices = new ConcurrentDictionary<long, ItemPrice>();

        public async Task CachePriceAsync()
        {
            var priceUrl = (await this.GetMotdAsync()).MarketPrices.HRef;
            try
            {
                if (_itemprices.Count > 0)
                    return;

                var result = await WebServiceConsumer.Consume<DefaultRestClient, JsonDeserializer, EveXmlSpecializedCollectionCacheAgent<ItemPriceCollection>, ItemPriceCollection>(priceUrl);
                result?.Items?.Foreach(x => _itemprices.TryAdd(x.ItemId.Id, x));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public double GetItemAveragePrice(long itemTypeId)
        {
            if (_itemprices.ContainsKey(itemTypeId))
                return _itemprices[itemTypeId].AveragePrice;

            return double.NaN;
        }

        public ItemPrice GetItemPrice(long itemTypeId)
        {
            if (_itemprices.ContainsKey(itemTypeId))
                return _itemprices[itemTypeId];

            return null;
        }

        #endregion Item Prices

        #region CharacterName

        private static CharacterNameCacheCollection _characterNames = new CharacterNameCacheCollection();

        public async Task<IEnumerable<CharacterName>> GetCharacterNamesAsync(long[] characterIds)
        {
            if (characterIds.Length == 0)
                return new CharacterName[0];

            // If not loaded... Load it
            try
            {
                if (_characterNames.Count == 0)
                    _characterNames.AddRange(await EveCache.GetCacheCollection<CharacterName>());
            }
            catch
            {
                // Happens... And expected if cache is not available... But too lazy to correctly handle this
            }

            var notFound = characterIds.Where(x => _characterNames.All(y => y.CharacterId != x));
            if (!notFound.Any() && _characterNames.Count > 0)
                return _characterNames.Where(x => characterIds.Contains(x.CharacterId));

            var result = await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, CharacterNameCollection>(
                 $"{this.Server.XMLApi}eve/CharacterName.xml.aspx?ids={(_characterNames.Count == 0 ? characterIds : notFound).Join(",")}");

            _characterNames.AddRange(result.Items);

            if (result.Items.Any())
                await EveCache.SetCache(_characterNames);

            return _characterNames.Where(x => characterIds.Contains(x.CharacterId));
        }

        private sealed class CharacterNameCacheCollection : ConcurrentKeyedCollection<long, CharacterName>
        {
            protected override long GetKeyForItem(CharacterName item) => item.CharacterId;
        }

        #endregion CharacterName

        #region Characters

        public async Task<IEnumerable<Character>> GetCharactersAsync(ApiKey apiKey)
        {
            var result = await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, EveXmlSpecializedCollectionCacheAgent<Character>, CharacterCollection>(
                $"{this.Server.XMLApi}account/Characters.xml.aspx?keyId={apiKey.KeyId}&vCode={apiKey.VerificationCode}");
            return result.Items;
        }

        #endregion Characters

        #region CharactersSheet

        public async Task<T> GetCharacterSheetAsync<T>(CharacterSpecificApiKey apiKey)
            where T : CharacterSheetCompact, new()
        {
            return await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, EveXmlSpecializedCacheAgent<T>, T>(
                $"{this.Server.XMLApi}char/CharacterSheet.xml.aspx?keyId={apiKey.KeyId}&vCode={apiKey.VerificationCode}&characterID={apiKey.CharacterId}");
        }

        #endregion CharactersSheet

        #region SkillInTraining

        public async Task<SkillInTraining> GetSkillInTrainingAsync(CharacterSpecificApiKey apiKey)
        {
            return await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, EveXmlSpecializedCacheAgent<SkillInTraining>, SkillInTraining>(
                $"{this.Server.XMLApi}char/SkillInTraining.xml.aspx?keyId={apiKey.KeyId}&vCode={apiKey.VerificationCode}&characterID={apiKey.CharacterId}");
        }

        #endregion SkillInTraining

        #region AssetItem

        public async Task<ReadOnlyDictionary<long, AssetItem>> GetAssetsAsync(CharacterSpecificApiKey apiKey)
        {
            var result = await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, EveXmlSpecializedCollectionCacheAgent<AssetItem>, AssetItemCollection>(
                 $"{this.Server.XMLApi}char/AssetList.xml.aspx?keyId={apiKey.KeyId}&vCode={apiKey.VerificationCode}&characterID={apiKey.CharacterId}");
            return result.Items.ToDictionary(x => x.ItemId).AsReadOnly();
        }

        #endregion AssetItem

        #region Wallet Journal

        public async Task<double> GetEstimatedAssetValue(CharacterSpecificApiKey apiKey)
        {
            await this.CachePriceAsync();
            var assets = await this.GetAssetsAsync(apiKey);
            return assets
                .Where(x => !x.Value.IsBlueprintCopy && !x.Value.IsCapsule && !x.Value.IsHiddenModifier && !x.Value.IsSkill)
                .Select(x => new { Price = this.GetItemPrice(x.Value.TypeId)?.AveragePrice })
                .Where(x => x.Price.HasValue)
                .Select(x => x.Price.Value)
                .Sum(x => x);
        }

        public async Task<double> GetEstimatedCharacterValue(CharacterSpecificApiKey apiKey)
        {
            var characterSheet = await this.GetCharacterSheetAsync<CharacterSheet>(apiKey);

            return (((DateTime.Now - characterSheet.DayOfBirth).TotalDays / 365.2425) * (this.GetItemAveragePrice(29668) / 3)) +
                    (characterSheet.Skills.Sum(x => x.Skillpoints) / 500000 * this.GetItemAveragePrice(40520));
        }

        public async Task<IEnumerable<WalletJournalItem>> GetWalletJournalAsync(CharacterSpecificApiKey apiKey)
        {
            var result = await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, EveXmlSpecializedCollectionCacheAgent<WalletJournal>, WalletJournal>(
                 $"{this.Server.XMLApi}char/WalletJournal.xml.aspx?keyId={apiKey.KeyId}&vCode={apiKey.VerificationCode}&characterID={apiKey.CharacterId}");
            return result.Items;
        }

        #endregion Wallet Journal

        #region Mail Message Item

        public async Task<IEnumerable<MailMessageItem>> GetMailMessagesAsync(CharacterSpecificApiKey apiKey)
        {
            var result = await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, MailMessageCollection>(
                 $"{this.Server.XMLApi}char/MailMessages.xml.aspx?keyId={apiKey.KeyId}&vCode={apiKey.VerificationCode}&characterID={apiKey.CharacterId}");
            return result.Items.Where(x => x.SenderId != apiKey.CharacterId);
        }

        #endregion Mail Message Item

        #region Notification

        public async Task<IEnumerable<Notification>> GetNotificationsAsync(CharacterSpecificApiKey apiKey)
        {
            var result = await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, NotificationCollection>(
                 $"{this.Server.XMLApi}char/Notifications.xml.aspx?keyId={apiKey.KeyId}&vCode={apiKey.VerificationCode}&characterID={apiKey.CharacterId}");
            return result.Items.Where(x => !x.IsRead);
        }

        #endregion Notification

        #region Mail Message Body

        public async Task<IEnumerable<MailBody>> GetMailBodyAsync(CharacterSpecificApiKey apiKey, long messageId)
        {
            var result = await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, EveXmlSpecializedCollectionCacheAgent<MailBodyCollection>, MailBodyCollection>(
                 $"{this.Server.XMLApi}char/MailBodies.xml.aspx?keyId={apiKey.KeyId}&vCode={apiKey.VerificationCode}&characterID={apiKey.CharacterId}&IDs={messageId}");
            return result.Items;
        }

        #endregion Mail Message Body

        #region Skill Training Queue

        public async Task<IEnumerable<SkillQueue>> GetSkillQueueAsync(CharacterSpecificApiKey apiKey)
        {
            var result = await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, EveXmlSpecializedCollectionCacheAgent<SkillQueueCollection>, SkillQueueCollection>(
                $"{this.Server.XMLApi}char/SkillQueue.xml.aspx?keyId={apiKey.KeyId}&vCode={apiKey.VerificationCode}&characterID={apiKey.CharacterId}");
            return result.Items;
        }

        #endregion Skill Training Queue

        #region Incursions

        public async Task<IncursionCollection> GetIncursionsAsync() =>
            await WebServiceConsumer.Consume<DefaultRestClient, JsonDeserializer, FiveMinutesDiskCacheAgent, IncursionCollection>((await this.GetMotdAsync()).Incursions.HRef);

        #endregion Incursions

        #region Solar Systems

        public async Task<CharacterLocationInfo> GetCharacterLocationAsync(long characterId)
        {
            var token = await this.GetAccessTokenAsync(characterId, CrestAccessScopes.CharacterLocationRead);

            return await WebServiceConsumer.Consume<DefaultRestClient, JsonDeserializer, CharacterLocationInfo>(
               $"{this.Server.Crest}characters/{characterId}/location/", token.AccessToken); // TODO
        }

        public async Task<SolarSystemInfo> GetSolarSystemInfoAsync(long solarSystemId) =>
            await WebServiceConsumer.Consume<DefaultRestClient, JsonDeserializer, SolarSystemInfo>(
                 $"{(await this.GetMotdAsync()).SolarSystems.HRef}{solarSystemId}/stats/");

        #endregion Solar Systems

        #region Ship fittings

        public async Task<ShipFittingCollection> GetShipFittings(long characterId)
        {
            var token = await this.GetAccessTokenAsync(characterId, CrestAccessScopes.CharacterFittingsRead);

            return await WebServiceConsumer.Consume<DefaultRestClient, JsonDeserializer, ShipFittingCollection>(
               $"{this.Server.Crest}characters/{characterId}/fittings/", token.AccessToken); // TODO
        }

        #endregion Ship fittings

        #region Open Window

        public async Task OpenMarketDetailsAsync(long characterId, long itemId)
        {
            var token = await this.GetAccessTokenAsync(characterId, CrestAccessScopes.RemoteClientUI);
            var body = new OpenMarketBody
            {
                Type = new OpenMarketBodyType
                {
                    Id = itemId,
                    HRef = $"{(await this.GetMotdAsync()).InventoryTypes.HRef}{itemId}/",
                }
            };
            await this.PostAsync($"{this.Server.Crest}characters/{characterId}/ui/openwindow/marketdetails/", body, token);
        }

        public async Task OpenOwnerDetailsAsync(long characterId, long id)
        {
            var token = await this.GetAccessTokenAsync(characterId, CrestAccessScopes.RemoteClientUI);
            await this.PostAsync($"{this.Server.Crest}characters/{characterId}/ui/openwindow/ownerdetails/", "{ \"id\":" + id + " }", token);
        }

        #endregion Open Window
    }
}