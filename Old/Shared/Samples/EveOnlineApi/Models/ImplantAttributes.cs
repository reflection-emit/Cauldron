using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace EveOnlineApi.Models
{
    public sealed class AttributeAttribute
    {
        [JsonProperty("attribute")]
        public AttributeDetails Attribute { get; internal set; }

        [JsonProperty("value")]
        public double Value { get; internal set; }
    }

    public sealed class AttributeDetails
    {
        [JsonProperty("id")]
        public long Id { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }
    }

    public sealed class Dogma
    {
        [JsonProperty("attributes")]
        public List<AttributeAttribute> Attributes { get; internal set; }
    }

    public sealed class ImplantAttributes : ItemAttributes
    {
        [JsonProperty("dogma")]
        public Dogma Dogma { get; internal set; }

        [JsonIgnore]
        public int SlotNumber
        {
            get
            {
                if (this.Dogma == null || (this.Dogma != null && this.Dogma.Attributes == null))
                    return -1;

                var attribute = this.Dogma.Attributes.FirstOrDefault(x => x.Attribute != null && x.Attribute.Name == "implantness");

                if (attribute == null)
                    return -1;

                return (int)attribute.Value;
            }
        }
    }
}