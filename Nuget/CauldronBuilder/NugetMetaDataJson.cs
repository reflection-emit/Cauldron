using Newtonsoft.Json;
using System;

namespace CauldronBuilder
{
    public class CatalogEntry
    {
        [JsonProperty("published")]
        public DateTime Published { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public class CatalogPage
    {
        [JsonProperty("items")]
        public Package[] Items { get; set; }
    }

    public class CatalogRoot
    {
        [JsonProperty("commitId")]
        public string ComitId { get; set; }

        [JsonProperty("items")]
        public CatalogPage[] Items { get; set; }
    }

    public class Package
    {
        [JsonProperty("catalogEntry")]
        public CatalogEntry CatalogEntry { get; set; }
    }
}