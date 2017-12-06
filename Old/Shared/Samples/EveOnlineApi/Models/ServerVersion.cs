using System.Runtime.Serialization;

namespace EveOnlineApi.Models
{
    [DataContract]
    public class ServerVersion
    {
        [DataMember]
        public string Version { get; set; }
    }
}