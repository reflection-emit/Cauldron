#r "nuget:Newtonsoft.Json,10.0.3"

using Newtonsoft.Json;
using System.IO;

public class CauldronBuilderData
{
    private static string path;

    private CauldronBuilderData()
    {
    }

    [JsonProperty("current-assembly-version")]
    public string CurrentAssemblyVersion { get; set; } = "2.0.0.0";

    [JsonProperty("current-package-version")]
    public string CurrentPackageVersion { get; set; } = "2.0.0";

    [JsonProperty("is-beta")]
    public bool IsBeta { get; set; } = true;

    public static CauldronBuilderData GetConfig(DirectoryInfo directoryInfo)
    {
        path = Path.Combine(directoryInfo.FullName, "meta-data.json");

        if (File.Exists(path))
            return JsonConvert.DeserializeObject<CauldronBuilderData>(File.ReadAllText(path));

        return new CauldronBuilderData();
    }

    public void IncrementAndSave()
    {
        this.CurrentAssemblyVersion = IncrementRevision(this.CurrentAssemblyVersion);
        this.CurrentPackageVersion = IncrementRevision(this.CurrentPackageVersion);

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