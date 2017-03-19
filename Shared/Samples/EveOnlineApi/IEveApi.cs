using EveOnlineApi.Models;
using EveOnlineApi.Models.StaticData;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EveOnlineApi
{
    public interface IEveApi
    {
        EveServers Server { get; }

        IEveStaticData StaticData { get; }

        Task AuthorizeApplicationForCrestAsync(CrestApiKey crest, params string[] accessScopes);

        Task CachePriceAsync();

        Task<ReadOnlyDictionary<long, AssetItem>> GetAssetsAsync(CharacterSpecificApiKey apiKey);

        Task<CharacterLocationInfo> GetCharacterLocationAsync(long characterId);

        Task<IEnumerable<CharacterName>> GetCharacterNamesAsync(long[] characterIds);

        Task<IEnumerable<Character>> GetCharactersAsync(ApiKey apiKey);

        Task<T> GetCharacterSheetAsync<T>(CharacterSpecificApiKey apiKey) where T : CharacterSheetCompact, new();

        Task<IEnumerable<ConquerableStation>> GetConquerableStationAsync();

        Task<ConquerableStation> GetConquerableStationAsync(long id);

        Task<string> GetConquerableStationNameAsync(long id);

        Task<double> GetEstimatedAssetValue(CharacterSpecificApiKey apiKey);

        Task<double> GetEstimatedCharacterValue(CharacterSpecificApiKey apiKey);

        Task<byte[]> GetImageAsync(ImageType type, long id, double daysToCache = 1, int imageSize = 256);

        Task<IncursionCollection> GetIncursionsAsync();

        Task<T> GetItemAttributesAsync<T>(long itemTypeId) where T : ItemAttributes, IKeyedModel, new();

        double GetItemAveragePrice(long itemTypeId);

        ItemPrice GetItemPrice(long itemTypeId);

        Task<IEnumerable<MailBody>> GetMailBodyAsync(CharacterSpecificApiKey apiKey, long messageId);

        Task<IEnumerable<MailMessageItem>> GetMailMessagesAsync(CharacterSpecificApiKey apiKey);

        Task<QuicklookCollection> GetMarketOrdersAsync(long itemTypeId);

        Task<IEnumerable<PriceHistory>> GetMarketPriceHistoryAsync(long itemTypeId, long regionId);

        Task<Motd> GetMotdAsync();

        Task<IEnumerable<Notification>> GetNotificationsAsync(CharacterSpecificApiKey apiKey);

        Task<ShipFittingCollection> GetShipFittings(long characterId);

        Task<SkillInTraining> GetSkillInTrainingAsync(CharacterSpecificApiKey apiKey);

        Task<IEnumerable<SkillQueue>> GetSkillQueueAsync(CharacterSpecificApiKey apiKey);

        Task<SolarSystemInfo> GetSolarSystemInfoAsync(long solarSystemId);

        Task<IEnumerable<WalletJournalItem>> GetWalletJournalAsync(CharacterSpecificApiKey apiKey);

        bool HasToken(long characterId, string accessScope);

        Task InitializeAsync(EveServers server, string clientId, string secretKey);

        Task OpenMarketDetailsAsync(long characterId, long itemId);

        Task OpenOwnerDetailsAsync(long characterId, long id);

        Task SaveAccessTokens(string clientId, string secretKey);
    }
}