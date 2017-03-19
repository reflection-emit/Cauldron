using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Extensions;
using EveOnlineApi.Exceptions;
using EveOnlineApi.Models;
using EveOnlineApi.Models.StaticData;
using EveOnlineApi.WebService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EveOnlineApi
{
    public sealed class EveStaticData : IEveStaticData
    {
        #region static data from resource

        private ReadOnlyDictionary<int, InventoryFlag> _inventoryFlags;
        private ReadOnlyDictionary<long, ItemGroup> _itemGroups;
        private ReadOnlyDictionary<long, ItemType> _itemTypes;
        private ReadOnlyDictionary<long, MarketGroup> _marketGroups;
        private ReadOnlyDictionary<long, NpcCorporation> _npcCorporations;
        private ReadOnlyDictionary<long, NpcStation> _npcStations;
        private bool staticDataLoaded = false;

        #endregion static data from resource

        internal EveStaticData()
        {
        }

        public bool CacheRebuildRequired { get; private set; }

        public ReadOnlyDictionary<long, ItemGroup> ItemGroups { get { return this._itemGroups; } }

        public ReadOnlyDictionary<long, ItemType> ItemTypes { get { return this._itemTypes; } }

        public async Task UpdateStaticDataAsync(bool force = false)
        {
            if (force)
            {
                await this.RebuildStaticDataAsync();
                return;
            }

            await this.LoadCache();

            var myVersion = (await Serializer.DeserializeAsync<ServerVersion>("ServerVersion"))?.Version;
            var serverVersion = await EveUtils.GetServerVersionAsync();

            if (myVersion != serverVersion || this.CacheRebuildRequired)
            {
                await this.RebuildStaticDataAsync();
                await Serializer.SerializeAsync(new ServerVersion { Version = serverVersion }, "ServerVersion");
            }
        }

        private async Task<List<TResult>> ConsumeUntil<TResult>(string requestUri, Func<TResult, string> nextUri)
        {
            List<TResult> items = new List<TResult>();
            TResult result;

            do
            {
                result = await WebServiceConsumer.Consume<DefaultRestClient, JsonDeserializer, TResult>(requestUri);
                requestUri = nextUri(result);

                items.Add(result);
            } while (requestUri != null);

            return items;
        }

        private async Task LoadCache()
        {
            this.LoadStaticData();

            try
            {
                // We are relying on an exception that happens here if the cache is not available
                await EveCache.GetCacheCollection<SolarSystem>();
                await EveCache.GetCacheCollection<Region>();
                await EveCache.GetCacheCollection<RefTypeItem>();

                this.CacheRebuildRequired = false;
            }
            catch
            {
                this.CacheRebuildRequired = true;
            }
        }

        private void LoadStaticData()
        {
            if (this.staticDataLoaded)
                return;

            var groups = this.LoadStaticResource<ItemGroup>("GroupId.bin");
            var itemTypes = this.LoadStaticResource<ItemType>("TypeId.bin");
            var marketGroups = this.LoadStaticResource<MarketGroup>("MarketGroup.bin");
            var npcCorporations = this.LoadStaticResource<NpcCorporation>("NPCCorporation.bin");
            var npcStations = this.LoadStaticResource<NpcStation>("NPCStation.bin");
            var inventoryFlag = this.LoadStaticResource<InventoryFlag>("InventoryFlag.bin");

            // Build the hierarchie full of wierd references :)
            foreach (var marketGroup in marketGroups)
            {
                // This has to stay null
                if (marketGroup.Id == marketGroup.ParentGroupId)
                    continue;

                marketGroup.Parent = marketGroups.FirstOrDefault(x => x.Id == marketGroup.ParentGroupId);
            }

            this._marketGroups = marketGroups.ToDictionary(x => x.Id).AsReadOnly();
            this._itemTypes = itemTypes.ToDictionary(x => x.Id).AsReadOnly();
            this._itemGroups = groups.ToDictionary(x => x.Id).AsReadOnly();
            this._npcCorporations = npcCorporations.ToDictionary(x => x.Id).AsReadOnly();
            this._npcStations = npcStations.ToDictionary(x => x.Id).AsReadOnly();
            this._inventoryFlags = inventoryFlag.ToDictionary(x => x.Id).AsReadOnly();

            this.staticDataLoaded = true;
        }

        private IEnumerable<T> LoadStaticResource<T>(string filename) =>
                    JsonConvert.DeserializeObject<IEnumerable<T>>(
                    Assemblies.GetManifestResource(filename)
                    .UncompressString());

        private async Task RebuildRefTypeItem()
        {
            var server = Factory.Create<IEveApi>().Server;
            var result = await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, RefTypeItemCollection>($"{server.XMLApi}eve/RefTypes.xml.aspx");
            await EveCache.SetCache(result.Items);
        }

        private async Task RebuildRegionsAndSolarSystems()
        {
            var eve = Factory.Create<IEveApi>();
            var regions = (await this.ConsumeUntil<RegionCollection>((await eve.GetMotdAsync()).Regions.HRef, x => x.Next == null ? null : x.Next.HRef)).SelectMany(x => x.Items);
            var solarSystems = (await this.ConsumeUntil<SolarSystemCollection>((await eve.GetMotdAsync()).SolarSystems.HRef, x => x.Next == null ? null : x.Next.HRef)).SelectMany(x => x.Items);

            await EveCache.SetCache(regions);
            await EveCache.SetCache(solarSystems);
        }

        private async Task RebuildStaticDataAsync()
        {
            if (!Network.HasInternetConnection || await EveUtils.GetServerStatusAsync() != EveServerStatus.Online)
                throw new EveServersOfflineException("Not connected to the internet or Eve servers are offline.");

            await this.RebuildRegionsAndSolarSystems();
            await this.RebuildRefTypeItem();

            this.solarSystemCache = null;
            this.regionCache = null;
            this.refTypeItemCache = null;
        }

        #region ItemTypes

        public ItemType GetItemType(long itemTypeId)
        {
            if (this._itemTypes.ContainsKey(itemTypeId))
                return this._itemTypes[itemTypeId];

            return null;
        }

        public long? GetItemTypeMarketGroupId(long itemTypeId)
        {
            if (this._itemTypes.ContainsKey(itemTypeId))
                return this._itemTypes[itemTypeId].MarketGroupId;

            return null;
        }

        public string GetItemTypeName(long itemTypeId)
        {
            if (this._itemTypes.ContainsKey(itemTypeId))
                return this._itemTypes[itemTypeId].Name;

            return string.Empty;
        }

        #endregion ItemTypes

        #region SolarSystems

        private ReadOnlyDictionary<long, SolarSystem> solarSystemCache;

        public ReadOnlyDictionary<long, SolarSystem> SolarSystems
        {
            get
            {
                if (this.solarSystemCache == null)
                    this.solarSystemCache = EveCache.GetCacheCollection<SolarSystem>().RunSync().ToDictionary(x => x.Id).AsReadOnly();

                return this.solarSystemCache;
            }
        }

        public string GetSolarSystemName(long systemId)
        {
            if (this.solarSystemCache.ContainsKey(systemId))
                return this.solarSystemCache[systemId].Name;

            return string.Empty;
        }

        #endregion SolarSystems

        #region ItemGroups

        public ItemGroup GetGroup(long id)
        {
            if (this._itemGroups.ContainsKey(id))
                return this._itemGroups[id];

            return null;
        }

        public string GetGroupName(long id)
        {
            if (this._itemGroups.ContainsKey(id))
                return this._itemGroups[id].Name;

            return string.Empty;
        }

        public string GetGroupNameFromTypeId(long itemTypeId)
        {
            var type = this.GetItemType(itemTypeId);

            if (type == null)
                return string.Empty;

            return type.GroupName;
        }

        #endregion ItemGroups

        #region Regions

        private ReadOnlyDictionary<long, Region> regionCache;

        public ReadOnlyDictionary<long, Region> Regions
        {
            get
            {
                if (this.regionCache == null)
                    this.regionCache = EveCache.GetCacheCollection<Region>().RunSync().OrderBy(x => x.Name).ToDictionary(x => x.Id).AsReadOnly();

                return this.regionCache;
            }
        }

        #endregion Regions

        #region NpcCorporations

        public ReadOnlyDictionary<long, NpcCorporation> NpcCorporations { get { return this._npcCorporations; } }

        public string GetNpcCorporationName(long id)
        {
            if (this._npcCorporations.ContainsKey(id))
                return this._npcCorporations[id].Description;

            return string.Empty;
        }

        #endregion NpcCorporations

        #region NpcStations

        public ReadOnlyDictionary<long, NpcStation> NpcStations { get { return this._npcStations; } }

        public string GetNpcStationName(long id)
        {
            if (this._npcStations.ContainsKey(id))
                return this._npcStations[id].Name;

            return string.Empty;
        }

        #endregion NpcStations

        #region RefTypeItem

        private ReadOnlyDictionary<long, RefTypeItem> refTypeItemCache;

        public ReadOnlyDictionary<long, RefTypeItem> RefTypeItems
        {
            get
            {
                if (this.refTypeItemCache == null)
                    this.refTypeItemCache = EveCache.GetCacheCollection<RefTypeItem>().RunSync().ToDictionary(x => x.TypeId).AsReadOnly();

                return this.refTypeItemCache;
            }
        }

        public string GetRefTypeItemName(long id)
        {
            if (this.refTypeItemCache.ContainsKey(id))
                return this.refTypeItemCache[id].TypeName;

            return string.Empty;
        }

        #endregion RefTypeItem

        #region Inventory Flags

        public ReadOnlyDictionary<int, InventoryFlag> InventoryFlags { get { return this._inventoryFlags; } }

        public string GetFlagDescription(int id)
        {
            if (this._inventoryFlags.ContainsKey(id))
                return this._inventoryFlags[id].Description;

            return string.Empty;
        }

        #endregion Inventory Flags

        #region Market Group Hierarchy

        public ReadOnlyDictionary<long, MarketGroup> MarketGroups { get { return this._marketGroups; } }

        public MarketGroup GetMarketGroup(long id)
        {
            if (this._marketGroups.ContainsKey(id))
                return this._marketGroups[id];

            return null;
        }

        public MarketGroup GetMarketGroupRoot(long id)
        {
            if (this._marketGroups.ContainsKey(id))
                return this._marketGroups[id].Root;

            return null;
        }

        #endregion Market Group Hierarchy
    }
}