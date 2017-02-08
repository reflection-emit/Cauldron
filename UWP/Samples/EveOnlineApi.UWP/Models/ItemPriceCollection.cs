using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections;

namespace EveOnlineApi.Models
{
    public sealed class ItemPriceCollection : ModelCollection<ItemPrice>, IXmlModelCollection
    {
        public ItemPriceCollection()
        {
            var myDateTime = DateTime.UtcNow;
            this.CachedUntil = myDateTime.Subtract(myDateTime.TimeOfDay).AddDays(1);
        }

        [JsonProperty("cachedUntil")]
        public DateTime CachedUntil { get; set; }

        IEnumerable IXmlModelCollection.Items
        {
            get { return this.Items; }
            set { this.Items = value.Cast<ItemPrice>().ToList<ItemPrice>(); }
        }

        [JsonProperty("key")]
        public string Key { get; set; }
    }
}