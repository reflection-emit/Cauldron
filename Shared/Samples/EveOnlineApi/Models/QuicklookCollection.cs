using Newtonsoft.Json;
using System.Collections.Generic;

namespace EveOnlineApi.Models
{
    public class QuicklookCollection
    {
        [XmlDeserializerNodePath("quicklook")]
        [XmlDeserializerElement("buy_orders", true)]
        [JsonProperty("buy_orders")]
        public List<QuicklookOrder> BuyOrders { get; internal set; }

        [XmlDeserializerNodePath("quicklook")]
        [XmlDeserializerElement("sell_orders", true)]
        [JsonProperty("sell_orders")]
        public List<QuicklookOrder> SellOrders { get; internal set; }
    }
}