namespace EveOnlineApi
{
    public class EveServers
    {
        static EveServers()
        {
            Tranquility = new EveServers
            {
                ApplicationManagement = "https://developers.eveonline.com/",
                Crest = "https://crest-tq.eveonline.com/",
                Image = "https://imageserver.eveonline.com/",
                Login = "https://login.eveonline.com/",
                XMLApi = "https://api.eveonline.com/",
                ServerName = "TRANQUILITY"
            };
            Singularity = new EveServers
            {
                ApplicationManagement = "https://developers.testeveonline.com/",
                Crest = "https://api-sisi.testeveonline.com/",
                Image = "https://image.testeveonline.com/",
                Login = "https://sisilogin.testeveonline.com/",
                XMLApi = "https://api.testeveonline.com/",
                ServerName = "SINGULARITY"
            };
        }

        private EveServers()
        {
        }

        public static EveServers Singularity { get; private set; }

        public static EveServers Tranquility { get; private set; }

        public string ApplicationManagement { get; private set; }

        public string Crest { get; private set; }

        public string Image { get; private set; }

        public string Login { get; private set; }

        public string ServerName { get; private set; }

        public string XMLApi { get; private set; }

        internal string ImageAllianceUrl { get { return this.Image + "Alliance/{0}_128.png"; } }

        internal string ImageCharacterUrl { get { return this.Image + "Character/{0}_{1}.jpg"; } }

        internal string ImageCorpUrl { get { return this.Image + "Corporation/{0}_128.png"; } }

        internal string ImageItemUrl { get { return this.Image + "Type/{0}_64.png"; } }
    }
}