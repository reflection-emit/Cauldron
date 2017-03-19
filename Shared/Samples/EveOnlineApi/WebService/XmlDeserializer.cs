namespace EveOnlineApi.WebService
{
    internal sealed class XmlDeserializer : IDeserializer
    {
        public TResult DeserializeObject<TResult>(string content)
        {
            var xml = new XmlDeserializer<TResult>(content);
            return xml.Deserialize();
        }
    }
}