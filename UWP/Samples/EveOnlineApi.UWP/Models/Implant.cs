using Cauldron.Activator;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{Name}")]
    public sealed class Implant
    {
        [JsonProperty("id")]
        public string Id
        {
            get { return this.Key + this.TypeId.ToString(); }
            set { /* Just dummy for the serializer */}
        }

        [JsonProperty("key")]
        public string Key { get; set; }

        [XmlDeserializerAttribute("typeName")]
        [JsonProperty("typeName")]
        public string Name { get; internal set; }

        [XmlDeserializerAttribute("typeID")]
        [JsonProperty("typeID")]
        public long TypeId { get; internal set; }

        public async Task<ImplantAttributes> GetAttributesAsync() =>
            await Factory.Create<IEveApi>().GetItemAttributesAsync<ImplantAttributes>(this.TypeId);
    }
}