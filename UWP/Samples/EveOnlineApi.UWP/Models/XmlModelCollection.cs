using Cauldron.IEnumerableExtensions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EveOnlineApi.Models
{
    public interface IXmlModelCollection
    {
        DateTime CachedUntil { get; set; }

        IEnumerable Items { get; set; }

        string Key { get; set; }
    }

    public abstract class XmlModelCollection<T> : IXmlModelCollection
    {
        [XmlDeserializerElement("cachedUntil")]
        [JsonProperty("cachedUntil")]
        public DateTime CachedUntil { get; set; }

        [XmlDeserializerNodePath("result")]
        [XmlDeserializerElement("rowset", true)]
        [JsonProperty("items")]
        public List<T> Items { get; set; }

        [JsonIgnore]
        IEnumerable IXmlModelCollection.Items
        {
            get { return this.Items; }
            set { this.Items = value.ToList_<T>(); }
        }

        [JsonProperty("key")]
        public string Key { get; set; }
    }
}