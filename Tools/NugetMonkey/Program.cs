using Cauldron;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace NugetMonkey
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args != null && args.Length == 2 && args[0] == "u")
            {
                UnlistPackagesAsync(args[1]).RunSync();
                return;
            }

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("This tool automagically retrieve all required nuget dependencies from the passed nuspec. It will clean rebuild all required projects, change version, pack and publish them.");
                return;
            }

            try
            {
                var path = Path.GetFullPath(args[0]);
                if (args.Length == 1 && Directory.Exists(path))
                    PackageNuspec(Directory.GetFiles(path, "*.nuspec"));
                else
                    PackageNuspec(args.Where(x => string.Equals(Path.GetExtension(args[0]), ".nuspec")).ToArray());
            }
            catch (Exception e)
            {
                WriteLine(e.GetStackTrace(), ConsoleColor.Red);
            }
        }

        private static void BuildNuGetPackage(ProjectInfo projectInfo)
        {
            Console.WriteLine($"Packing: {projectInfo.NuspecPath}");

            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(NugetMonkeyJson.Nugetpath),
                FileName = NugetMonkeyJson.Nugetpath,
                Arguments = $"pack \"{projectInfo.NuspecPath}\" -Symbols -OutputDir \"{NugetMonkeyJson.NugetOutputPath}\"",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(startInfo);
            var standard = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            process.WaitForExit();
            Console.WriteLine(standard);

            if (process.ExitCode != 0)
                throw new Exception(error);
        }

        private static void BuildProject(Project1 project)
        {
            Parallel.ForEach(project.Buildconfig, build =>
            {
                Console.WriteLine($"Building: {project.Path} | {build}");

                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(NugetMonkeyJson.Solutionpath),
                    FileName = NugetMonkeyJson.Msbuildpath,
                    Arguments = $"\"{project.Path}\" /target:Clean;Rebuild /p:Configuration={build} /m:8 /nr:false",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var process = Process.Start(startInfo);
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                    throw new Exception(output);
            });
        }

        private static void IncrementVersion(ProjectInfo project)
        {
            var version = project.ProjectVersion.Increment();
            Parallel.ForEach(project.Infos, p =>
            {
                var projectFileBody = File.ReadAllText(p.Path);
                if (projectFileBody.StartsWith(@"<Project Sdk=""Microsoft.NET.Sdk"">")) // This is a NetStandard project
                {
                    var packageVersion = projectFileBody.EnclosedIn("<Version>", "</Version>");
                    if (packageVersion != null)
                        projectFileBody = projectFileBody.Replace(packageVersion, $"<Version>{version}</Version>");

                    var assemblyVersion = projectFileBody.EnclosedIn("<AssemblyVersion>", "</AssemblyVersion>");
                    if (assemblyVersion != null)
                        projectFileBody = projectFileBody.Replace(assemblyVersion, $"<AssemblyVersion>{version}</AssemblyVersion>");
                    else
                        projectFileBody = Insert(projectFileBody, $"<AssemblyVersion>{version}</AssemblyVersion>\r\n");

                    var fileVersion = projectFileBody.EnclosedIn("<FileVersion>", "</FileVersion>");
                    if (fileVersion != null)
                        projectFileBody = projectFileBody.Replace(fileVersion, $"<FileVersion>{version}</FileVersion>");
                    else
                        projectFileBody = Insert(projectFileBody, $"<FileVersion>{version}</FileVersion>\r\n");

                    File.WriteAllText(p.Path, projectFileBody);
                }
                else
                {
                    // Lets look for AssemblyInfo.cs
                    var assemblyInfo = Path.Combine(Path.GetDirectoryName(p.Path), "Properties\\AssemblyInfo.cs");
                    if (!File.Exists(assemblyInfo))
                    {
                        assemblyInfo = Directory.GetFiles(Path.GetDirectoryName(p.Path), "AssemblyInfo.cs", SearchOption.AllDirectories).FirstOrDefault();
                        if (assemblyInfo == null)
                            return;
                    }

                    var assemblyInfoBody = File.ReadAllText(assemblyInfo);

                    var assemblyVersion = Regex.Match(assemblyInfoBody, @"\[assembly: AssemblyVersion\(""(.*?)""\)\]").Groups[0].Value;
                    if (assemblyVersion != null)
                        assemblyInfoBody = assemblyInfoBody.Replace(assemblyVersion, $"[assembly: AssemblyVersion(\"{version}\")]");

                    var fileVersion = Regex.Match(assemblyInfoBody, @"\[assembly: AssemblyFileVersion\(""(.*?)""\)\]").Groups[0].Value;
                    if (fileVersion != null)
                        assemblyInfoBody = assemblyInfoBody.Replace(fileVersion, $"[assembly: AssemblyFileVersion(\"{version}\")]");

                    File.WriteAllText(assemblyInfo, assemblyInfoBody);
                }
            });
        }

        private static string Insert(string body, string data)
        {
            int position = body.IndexOf("</PropertyGroup>");

            if (position < 0)
                return body;

            return body.Insert(position, data);
        }

        private static void ModifyNuspec(ProjectInfo projectInfo)
        {
            Console.WriteLine($"Modifying: {projectInfo.Id} -> {projectInfo.NugetVersion} to {projectInfo.NugetVersion.Increment()}");

            var nuspec = new XmlDocument();
            nuspec.Load(projectInfo.NuspecPath);
            var dependencies = nuspec["package"]["metadata"]["dependencies"];

            foreach (XmlElement item in dependencies.Contains("group") ? dependencies["group"] : dependencies)
            {
                var id = item.Attributes["id"].Value;
                if (ProjectInfo.ProjectInfos.TryGetValue(id, out ProjectInfo info))
                    item.Attributes["version"].Value = info.NugetVersion.Increment().ToString();
            }

            if (!string.IsNullOrEmpty(NugetMonkeyJson.Owners)) nuspec["package"]["metadata"]["owners"].InnerText = NugetMonkeyJson.Owners;
            if (!string.IsNullOrEmpty(NugetMonkeyJson.Authors)) nuspec["package"]["metadata"]["authors"].InnerText = NugetMonkeyJson.Authors;
            if (NugetMonkeyJson.RequireLicenseAcceptance.HasValue) nuspec["package"]["metadata"]["requireLicenseAcceptance"].InnerText = NugetMonkeyJson.RequireLicenseAcceptance.Value ? "true" : "false";
            if (!string.IsNullOrEmpty(NugetMonkeyJson.LicenseUrl)) nuspec["package"]["metadata"]["licenseUrl"].InnerText = NugetMonkeyJson.LicenseUrl;
            if (!string.IsNullOrEmpty(NugetMonkeyJson.ProjectUrl)) nuspec["package"]["metadata"]["projectUrl"].InnerText = NugetMonkeyJson.ProjectUrl;
            if (!string.IsNullOrEmpty(NugetMonkeyJson.IconUrl)) nuspec["package"]["metadata"]["iconUrl"].InnerText = NugetMonkeyJson.IconUrl;
            if (!string.IsNullOrEmpty(NugetMonkeyJson.Copyright)) nuspec["package"]["metadata"]["copyright"].InnerText = NugetMonkeyJson.Copyright;

            nuspec["package"]["metadata"]["id"].InnerText = Path.GetFileNameWithoutExtension(projectInfo.NuspecPath);
            nuspec["package"]["metadata"]["version"].InnerText = projectInfo.NugetVersion.Increment().ToString();

            nuspec.Save(projectInfo.NuspecPath);
        }

        private static void PackageNuspec(string[] nuspecPaths)
        {
            if (!nuspecPaths.Any())
            {
                WriteLine("No nuspec file in parameters", ConsoleColor.Red);
                return;
            }

            var projectInfos = nuspecPaths.Select(x => new ProjectInfo(x)).ToArray();

            WriteLine("Getting all projects that is required for the package.");
            var dependencies = projectInfos
                .Concat(projectInfos.SelectMany(x => x.GetDependencies()))
                .Distinct()
                .OrderBy(x => x.Project.Priority);

            foreach (var item in dependencies)
                Console.WriteLine($"{item} {item.ProjectVersion} {item.NugetVersion}");

            WriteLine("Getting nuget information from nuget.org", ConsoleColor.Cyan);
            Task.WhenAll(dependencies.Select(x => x.GetNugetInfo())).Wait();

            WriteLine("Incrementing the project versions", ConsoleColor.Cyan);

            foreach (var item in dependencies)
                IncrementVersion(item);

            WriteLine("Building projects");

            foreach (var project in dependencies)
                foreach (var item in project.Infos)
                    BuildProject(item);

            WriteLine("Modifying nuspecs");
            foreach (var project in dependencies)
                ModifyNuspec(project);

            foreach (var item in Directory.GetFiles(NugetMonkeyJson.NugetOutputPath))
                File.Delete(item);

            WriteLine("Packing nuspecs");
            Parallel.ForEach(dependencies, project => BuildNuGetPackage(project));

            WriteLine("Do you want to upload the packages? [Y/N]", ConsoleColor.Magenta);
            if (Console.ReadKey().Key == ConsoleKey.Y)
                Parallel.ForEach(
                    Directory.GetFiles(NugetMonkeyJson.NugetOutputPath, "*.nupkg").Where(x => x.IndexOf(".symbols", StringComparison.InvariantCultureIgnoreCase) < 0),
                    file => UploadNugetPackage(file));
        }

        private static void UploadNugetPackage(string nupkgPath)
        {
            Console.WriteLine($"Uploading: {nupkgPath}");

            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = Path.GetDirectoryName(NugetMonkeyJson.Nugetpath);
            startInfo.FileName = NugetMonkeyJson.Nugetpath;
            startInfo.Arguments = $"push \"{nupkgPath}\" -Source https://www.nuget.org/api/v2/package";
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            var process = Process.Start(startInfo);
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            process.WaitForExit();
            Console.WriteLine(output);
            Console.WriteLine(error);

            if (process.ExitCode != 0)
                throw new Exception(error);
        }

        private static void WriteLine(string text, ConsoleColor color = ConsoleColor.Green)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        #region Unlister by JGauffin http://blog.gauffin.org/2016/09/how-to-remove-a-package-from-nuget-org/

        private static async Task<IEnumerable<string>> GetListedPackageVersionsAsync(string packageID)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync($"https://api.nuget.org/v3/registration3/{packageID.ToLower()}/index.json");
                return JsonConvert.DeserializeObject<CatalogRoot>(response)
                    .Items
                    .SelectMany(x => x.Items)
                    .Select(x => x.CatalogEntry)
                    .Select(x => x.Version)
                    .Distinct();
            }
        }

        private static string UnlistPackage(string packageId, string packageVersion)
        {
            var arguments = $"delete {packageId} {packageVersion} -NonInteractive -Source https://www.nuget.org/api/v2/package";
            var processInfo = new ProcessStartInfo(NugetMonkeyJson.Nugetpath, arguments)
            {
                RedirectStandardOutput = true,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                UseShellExecute = false
            };
            var process = Process.Start(processInfo);
            Console.WriteLine(arguments);
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd();
        }

        private static async Task UnlistPackagesAsync(string packageId)
        {
            var versions = await GetListedPackageVersionsAsync(packageId);

            foreach (var version in versions)
            {
                var output = UnlistPackage(packageId, version);
                Console.WriteLine(output);
            }
        }

        #endregion Unlister by JGauffin http://blog.gauffin.org/2016/09/how-to-remove-a-package-from-nuget-org/
    }
}