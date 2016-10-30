using EveOnlineApi.Models.StaticData;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EveOnlineApi
{
    public interface IEveStaticData
    {
        bool CacheRebuildRequired { get; }

        ReadOnlyDictionary<int, InventoryFlag> InventoryFlags { get; }

        ReadOnlyDictionary<long, ItemGroup> ItemGroups { get; }

        ReadOnlyDictionary<long, ItemType> ItemTypes { get; }

        ReadOnlyDictionary<long, MarketGroup> MarketGroups { get; }

        ReadOnlyDictionary<long, NpcCorporation> NpcCorporations { get; }

        ReadOnlyDictionary<long, NpcStation> NpcStations { get; }

        ReadOnlyDictionary<long, RefTypeItem> RefTypeItems { get; }

        ReadOnlyDictionary<long, Region> Regions { get; }

        ReadOnlyDictionary<long, SolarSystem> SolarSystems { get; }

        string GetFlagDescription(int id);

        ItemGroup GetGroup(long id);

        string GetGroupName(long id);

        string GetGroupNameFromTypeId(long itemTypeId);

        ItemType GetItemType(long itemTypeId);

        long? GetItemTypeMarketGroupId(long itemTypeId);

        string GetItemTypeName(long itemTypeId);

        MarketGroup GetMarketGroup(long id);

        MarketGroup GetMarketGroupRoot(long id);

        string GetNpcCorporationName(long id);

        string GetNpcStationName(long id);

        string GetRefTypeItemName(long id);

        string GetSolarSystemName(long systemId);

        Task UpdateStaticDataAsync(bool force = false);
    }
}