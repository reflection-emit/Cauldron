using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace NugetMonkey
{
    public sealed class ProjectInfo : IEquatable<ProjectInfo>
    {
        private Version _projectVersion;
        private string nugetDirectory;
        private string nugetPath;
        private XmlDocument nuspec;

        public ProjectInfo(string nugetPath)
        {
            this.nugetDirectory = Path.GetDirectoryName(nugetPath);
            this.nugetPath = nugetPath;
            this.nuspec = new XmlDocument();
            this.nuspec.Load(nugetPath);

            this.Id = this.nuspec["package"]["metadata"]["id"].InnerText;
            this.Project = NugetMonkeyJson.ProjectInfo[this.Id];
            this.Infos = this.Project.Projects;
            this.ProjectName = Path.GetFileNameWithoutExtension(this.Infos[0].Path);
            this.NugetVersion = this.nuspec["package"]["metadata"]["version"].InnerText;
            this.ProjectVersion = GetProjectVersion(this.Infos[0].Path);

            ProjectInfo.ProjectInfos.TryAdd(this.Id, this);
        }

        public static ConcurrentDictionary<string, ProjectInfo> ProjectInfos { get; } = new ConcurrentDictionary<string, ProjectInfo>();

        public string Id { get; }

        public Project1[] Infos { get; private set; }

        public NugetVersion NugetVersion { get; private set; }

        public string NuspecPath => this.nugetPath;

        public CatalogRoot PackageInfo { get; private set; }

        public Project Project { get; }

        public string ProjectName { get; private set; }

        public Version ProjectVersion
        {
            get
            {
                if (this.PackageInfo == null)
                    return this._projectVersion;

                return this.NugetVersion;
            }
            set { this._projectVersion = value; }
        }

        public bool Equals(ProjectInfo other) => other.ProjectName == this.ProjectName;

        public IEnumerable<ProjectInfo> GetDependencies()
        {
            var dependencies = nuspec["package"]["metadata"]["dependencies"];

            foreach (XmlElement item in dependencies.Contains("group") ? dependencies["group"] : dependencies)
            {
                var id = item.Attributes["id"].Value;
                if (NugetMonkeyJson.ProjectInfo.TryGetValue(id, out Project project))
                {
                    if (ProjectInfos.TryGetValue(id, out ProjectInfo projectInfo))
                    {
                        yield return projectInfo;

                        foreach (var d in projectInfo.GetDependencies())
                            yield return d;
                    }
                    else
                    {
                        if (NugetMonkeyJson.ProjectInfo.ContainsKey(id))
                        {
                            var newProjectInfo = new ProjectInfo(Path.Combine(this.nugetDirectory, id + ".nuspec"));
                            ProjectInfos.TryAdd(id, newProjectInfo);
                            yield return newProjectInfo;

                            foreach (var d in newProjectInfo.GetDependencies())
                                yield return d;
                        }
                    }
                }
            }
        }

        public override int GetHashCode() => this.ProjectName.GetHashCode();

        public async Task GetNugetInfo()
        {
            try
            {
                Console.WriteLine($"Getting package info: {this.Id}");
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://api.nuget.org/v3/registration3/{this.Id.ToLower()}/index.json");

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        this.PackageInfo = JsonConvert.DeserializeObject<CatalogRoot>(data);
                        this.NugetVersion = new NugetVersion(this.PackageInfo.Items.Last().Items.Last().CatalogEntry.Version);
                    }
                }
            }
            catch
            {
                // Nothing to see here
            }
        }

        public override string ToString() => this.ProjectName;

        private static Version GetProjectVersion(string projectPath)
        {
            var projectFileBody = File.ReadAllText(projectPath);
            if (projectFileBody.StartsWith(@"<Project Sdk=""Microsoft.NET.Sdk"">")) // This is a NetStandard project
            {
                var assemblyVersion = Regex.Match(projectFileBody, "<AssemblyVersion>(.*?)</AssemblyVersion>").Groups[1].Value;
                return assemblyVersion == null ? new Version(1, 0, 0, 0) : new Version(assemblyVersion);
            }
            else
            {
                // Lets look for AssemblyInfo.cs
                var assemblyInfo = Path.Combine(Path.GetDirectoryName(projectPath), "Properties\\AssemblyInfo.cs");
                if (!File.Exists(assemblyInfo))
                {
                    assemblyInfo = Directory.GetFiles(Path.GetDirectoryName(projectPath), "AssemblyInfo.cs", SearchOption.AllDirectories).FirstOrDefault();
                    if (assemblyInfo == null)
                        return new Version(1, 0, 0, 0);
                }

                var assemblyInfoBody = File.ReadAllText(assemblyInfo);

                var assemblyVersion = Regex.Match(assemblyInfoBody, @"\[assembly: AssemblyVersion\(""(.*?)""\)\]").Groups[1].Value;

                if (assemblyVersion.IndexOf('*') >= 0)
                    return new Version(1, 0, 0, 0);

                return assemblyVersion == null ? new Version(1, 0, 0, 0) : new Version(assemblyVersion);
            }
        }
    }
}