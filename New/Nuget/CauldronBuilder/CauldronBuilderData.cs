using Newtonsoft.Json;
using System.IO;

namespace CauldronBuilder
{
    public class CauldronBuilderData
    {
        private CauldronBuilderData()
        {
        }

        public static CauldronBuilderData Current
        {
            get
            {
                var path = Path.Combine(Path.GetDirectoryName(typeof(CauldronBuilderData).Assembly.Location), "meta-data.json");

                if (File.Exists(path))
                    return JsonConvert.DeserializeObject<CauldronBuilderData>(File.ReadAllText(path));

                return new CauldronBuilderData();
            }
        }

        [JsonProperty("current-assembly-version")]
        public string CurrentAssemblyVersion { get; set; } = "2.0.0.0";

        [JsonProperty("current-package-version")]
        public string CurrentPackageVersion { get; set; } = "2.0.0";

        [JsonProperty("is-beta")]
        public bool IsBeta { get; set; } = true;

        public void IncrementAndSave()
        {
            this.CurrentAssemblyVersion = IncrementRevision(this.CurrentAssemblyVersion);
            this.CurrentPackageVersion = IncrementRevision(this.CurrentPackageVersion);

            var path = Path.Combine(Path.GetDirectoryName(typeof(CauldronBuilderData).Assembly.Location), "meta-data.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(this));
        }

        private string IncrementRevision(string version)
        {
            var data = version.Split('.');
            var revision = data[data.Length - 1];
            int casted;

            if (!int.TryParse(revision, out casted))
                casted = 0;

            data[data.Length - 1] = (++casted).ToString();

            return string.Join('.', data);
        }
    }
}