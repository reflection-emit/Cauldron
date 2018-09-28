using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NugetMonkey
{
    public static class NugetMonkeyJson
    {
        private static Rootobject rootobject;

        static NugetMonkeyJson()
        {
            rootobject = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(typeof(NugetMonkeyJson).Assembly.Location), "nugetmonkey.json")));
            ProjectInfo = rootobject.Projects.ToDictionary(x => x.Id, x => x);
        }

        public static Dictionary<string, Project> ProjectInfo { get; }
        public static string Authors => rootobject.Authors;
        public static string BasicVersion => rootobject.BasicVersion;
        public static string Copyright => rootobject.Copyright;
        public static string IconUrl => rootobject.IconUrl;
        public static string LicenseUrl => rootobject.LicenseUrl;
        public static string Msbuildpath => rootobject.Msbuildpath;
        public static string NugetOutputPath => rootobject.NugetOutputPath;
        public static string Nugetpath => rootobject.Nugetpath;
        public static string Owners => rootobject.Owners;
        public static Project[] Projects => rootobject.Projects;
        public static string ProjectUrl => rootobject.ProjectUrl;
        public static bool? RequireLicenseAcceptance => rootobject.RequireLicenseAcceptance;
        public static string Solutionpath => rootobject.Solutionpath;
    }

    public class Project
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("priority")]
        public int Priority { get; set; }

        [JsonProperty("projects")]
        public Project1[] Projects { get; set; }
    }

    public class Project1
    {
        private string _path;

        [JsonProperty("build-config")]
        public string[] Buildconfig { get; set; }

        [JsonProperty("path")]
        public string Path
        {
            get => this._path;
            set => this._path = System.IO.Path.GetFullPath(value);
        }
    }

    public class Rootobject
    {
        private string _msbuildpath;
        private string _nugetOutputPath;
        private string _nugetpath;

        private string _solutionPath;

        [JsonProperty("authors")]
        public string Authors { get; set; }

        [JsonProperty("basic-version")]
        public string BasicVersion { get; set; }

        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; }

        [JsonProperty("licenseUrl")]
        public string LicenseUrl { get; set; }

        [JsonProperty("msbuild-path")]
        public string Msbuildpath
        {
            get => this._msbuildpath;
            set => this._msbuildpath = System.IO.Path.GetFullPath(value);
        }

        [JsonProperty("nuget-output-path")]
        public string NugetOutputPath
        {
            get => this._nugetOutputPath;
            set => this._nugetOutputPath = System.IO.Path.GetFullPath(value);
        }

        [JsonProperty("nuget-path")]
        public string Nugetpath
        {
            get => this._nugetpath;
            set => this._nugetpath = System.IO.Path.GetFullPath(value);
        }

        [JsonProperty("owners")]
        public string Owners { get; set; }

        [JsonProperty("projects")]
        public Project[] Projects { get; set; }

        [JsonProperty("projectUrl")]
        public string ProjectUrl { get; set; }

        [JsonProperty("requireLicenseAcceptance")]
        public bool? RequireLicenseAcceptance { get; set; }

        [JsonProperty("solution-path")]
        public string Solutionpath
        {
            get => this._solutionPath;
            set => this._solutionPath = System.IO.Path.GetFullPath(value);
        }
    }
}