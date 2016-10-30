using Newtonsoft.Json;

namespace EveOnlineApi.WebService
{
    internal sealed class JsonDeserializer : IDeserializer
    {
        public TResult DeserializeObject<TResult>(string content)
        {
            return JsonConvert.DeserializeObject<TResult>(content);
        }
    }
}