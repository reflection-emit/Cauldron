using Cauldron;
using Cauldron.Cryptography;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Win32_Console_ParameterHandling
{
    public static class EveApi
    {
        private static Motd GetMotd => Get<Motd>("https://crest-tq.eveonline.com/");

        public static IEnumerable<ItemPrice> GetPrices()
        {
            var result = Get<ItemPriceCollection>(GetMotd.MarketPrices.HRef);
            return result.Items;
        }

        private static T Get<T>(string requestUri)
        {
            var hash = requestUri.GetHash(HashAlgorithms.Md5);
            var cache = Path.Combine(Path.GetTempPath(), hash);

            if (File.Exists(cache))
            {
                if (File.GetLastWriteTime(cache) < DateTime.Now.AddMinutes(-5))
                    File.Delete(cache);
                else
                    return JsonConvert.DeserializeObject<T>(File.ReadAllText(cache));
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(requestUri).RunSync();

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                        case HttpStatusCode.Accepted:

                            var @string = response.Content.ReadAsStringAsync().RunSync();
                            File.WriteAllText(cache, @string);

                            return JsonConvert.DeserializeObject<T>(@string);

                        case HttpStatusCode.NotImplemented:
                            throw new NotImplementedException(response.ReasonPhrase);

                        case HttpStatusCode.RequestTimeout:
                            throw new TimeoutException(response.ReasonPhrase);

                        case HttpStatusCode.Unauthorized:
                            throw new UnauthorizedAccessException(response.ReasonPhrase);

                        default:
                            throw new Exception(response.ReasonPhrase);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public sealed class ItemPrice
        {
            [JsonProperty("adjustedPrice")]
            public double AdjustedPrice { get; internal set; }

            [JsonProperty("averagePrice")]
            public double AveragePrice { get; internal set; }

            [JsonProperty("type")]
            public ItemType Type { get; internal set; }
        }

        public sealed class ItemType
        {
            [JsonProperty("id")]
            public int Id { get; internal set; }

            [JsonProperty("name")]
            public string Name { get; internal set; }
        }

        private sealed class ItemPriceCollection
        {
            [JsonProperty("items")]
            public IEnumerable<ItemPrice> Items { get; internal set; }
        }

        private sealed class Motd
        {
            [JsonProperty("marketPrices")]
            public Next MarketPrices { get; private set; }
        }

        private class Next
        {
            [JsonProperty("href")]
            public string HRef { get; internal set; }
        }
    }
}