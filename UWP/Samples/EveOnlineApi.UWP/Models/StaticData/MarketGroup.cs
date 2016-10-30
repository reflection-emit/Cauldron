using Newtonsoft.Json;
using System.Diagnostics;

namespace EveOnlineApi.Models.StaticData
{
    [DebuggerDisplay("{Name}")]
    public sealed class MarketGroup
    {
        private MarketGroup _root;

        [JsonProperty("description")]
        public string Description { get; internal set; }

        [JsonProperty("hasTypes")]
        public bool HasTypes { get; internal set; }

        [JsonProperty("id")]
        public long Id { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonIgnore]
        public MarketGroup Parent { get; internal set; }

        [JsonProperty("parentId")]
        public long ParentGroupId { get; internal set; }

        [JsonIgnore]
        public MarketGroup Root
        {
            get
            {
                // I am the root
                if (this.Parent == null)
                    return null;

                if (this._root == null)
                {
                    var parent = this.Parent;

                    do
                    {
                        this._root = parent;
                        parent = this._root.Parent;
                    } while (parent != null);
                }

                return this._root;
            }
        }
    }
}