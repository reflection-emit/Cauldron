using Newtonsoft.Json;
using System;

namespace EveOnlineApi.Models
{
    public sealed class QuicklookBuyOrder : QuicklookOrder
    {
    }

    public class QuicklookOrder
    {
        [XmlDeserializerElement("expires")]
        [JsonProperty("expires")]
        public DateTime Expires { get; internal set; }

        [JsonProperty("isSellOrder")]
        public bool IsSellOrder { get; internal set; }

        [XmlDeserializerElement("min_volume")]
        [JsonProperty("min_volume")]
        public long MinimumVolume { get; internal set; }

        [XmlDeserializerElement("price")]
        [JsonProperty("price")]
        public double Price { get; internal set; }

        [XmlDeserializerElement("region")]
        [JsonProperty("region")]
        public long RegionId { get; internal set; }

        [XmlDeserializerElement("vol_remain")]
        [JsonProperty("vol_remain")]
        public long RemainingVolume { get; internal set; }

        [XmlDeserializerElement("security")]
        [JsonProperty("security")]
        public float Security { get; internal set; }

        [XmlDeserializerElement("station")]
        [JsonProperty("station")]
        public long StationId { get; internal set; }

        [XmlDeserializerElement("station_name")]
        [JsonProperty("station_name")]
        public string StationName { get; internal set; }
    }

    public sealed class QuicklookSellOrder : QuicklookOrder
    {
        public QuicklookSellOrder()
        {
            this.IsSellOrder = true;
        }
    }
}