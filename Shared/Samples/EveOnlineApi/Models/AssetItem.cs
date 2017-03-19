using EveOnlineApi.Models.StaticData;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{TypeName}")]
    public sealed class AssetItem
    {
        private ItemType _itemType;
        private Location _location;

        [JsonProperty("containerName")]
        public string ContainerName { get; internal set; }

        [XmlDeserializerAttribute("flag")]
        [JsonProperty("flag")]
        public int Flag { get; internal set; }

        [JsonIgnore]
        public bool IsBlueprint
        {
            get { return this.ItemType != null && this.ItemType.MarketGroup != null && this.ItemType.MarketGroup.Root.Id == 2; }
        }

        [JsonIgnore]
        public bool IsBlueprintCopy
        {
            get { return this.IsBlueprint && this.RawQuantity == -2; }
        }

        [JsonIgnore]
        public bool IsCapsule
        {
            get { return this.Flag == 56; }
        }

        [JsonIgnore]
        public bool IsHiddenModifier
        {
            get { return this.Flag == 156; }
        }

        [JsonIgnore]
        public bool IsImplant
        {
            get { return this.Flag == 89; }
        }

        [JsonIgnore]
        public bool IsInsideShip
        {
            get
            {
                return (this.Flag >= 11 && this.Flag <= 35) || this.Flag == 5 || this.Flag == 87 ||
                    (this.Flag >= 92 && this.Flag <= 99) || (this.Flag >= 125 && this.Flag <= 143)
                     || this.Flag == 48 || this.Flag == 49 || this.Flag == 151 || this.Flag == 154
                      || (this.Flag >= 158 && this.Flag <= 163);
            }
        }

        [JsonIgnore]
        public bool IsLocked
        {
            get { return this.Flag == 63; }
        }

        [XmlDeserializerAttribute("singleton")]
        [JsonProperty("singleton")]
        public bool IsNotPackaged { get; internal set; }

        [JsonIgnore]
        public bool IsShip
        {
            get { return this.ItemType.MarketGroup.Root.Id == 4; }
        }

        [JsonIgnore]
        public bool IsSkill
        {
            get { return this.Flag == 7 || this.Flag == 61; }
        }

        [JsonIgnore]
        public bool IsStructure
        {
            get { return this.ItemType.MarketGroup.Root.Id == 477; }
        }

        [JsonIgnore]
        public bool IsUnlocked
        {
            get { return this.Flag == 64; }
        }

        [XmlDeserializerAttribute("itemID")]
        [JsonProperty("itemId")]
        public long ItemId { get; internal set; }

        [JsonIgnore]
        public ItemType ItemType
        {
            get
            {
                if (this._itemType == null)
                    this._itemType = Api.Current.StaticData.GetItemType(this.TypeId);

                return this._itemType;
            }
        }

        [JsonIgnore]
        public Location Location
        {
            get
            {
                if (this._location == null)
                    this._location = new Location(this.LocationId);

                return this._location;
            }
        }

        [XmlDeserializerAttribute("locationID")]
        [JsonProperty("locationID")]
        public long LocationId { get; internal set; }

        [XmlDeserializerAttribute("quantity")]
        [JsonProperty("quantity")]
        public long Quantity { get; internal set; }

        [XmlDeserializerAttribute("rawQuantity")]
        [JsonProperty("rawQuantity")]
        public long RawQuantity { get; internal set; }

        [XmlDeserializerAttribute("typeID")]
        [JsonProperty("typeID")]
        public long TypeId { get; internal set; }

        [JsonIgnore]
        public string TypeName
        {
            get
            {
                return this.ItemType == null ? string.Empty : this.ItemType.Name;
            }
        }

        public async Task<string> GetLocationNameAsync(long id)
        {
            // TODO
            var result = await this.Location.GetLocationNameAsync(id);

            if (string.IsNullOrEmpty(result))
            {
                if (this.IsInsideShip || this.IsUnlocked || this.IsLocked) /* Is inside a ship or a container */
                    return this.ContainerName;
                else
                    return Api.Current.StaticData.GetFlagDescription(this.Flag);
            }

            return result;
        }

        public override string ToString()
        {
            if (this.IsBlueprint && this.IsBlueprintCopy)
                return this.TypeName + " (Copy)";
            else if (this.IsBlueprint)
                return this.TypeName + " (Original)";
            else
                return this.TypeName;
        }
    }
}