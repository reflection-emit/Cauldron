using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CauldronBuilder
{
    internal static class Program
    {
        private const string Authors = "Alexander Schunk, Capgemini Deutschland GmbH";
        private const string Copyright = "Copyright (c) 2016 Capgemini Deutschland GmbH";
        private static CauldronBuilderData data;

        public static void Main(string[] args)
        {
            var startingLocation = new DirectoryInfo(args == null || args.Length == 0 ? Path.GetDirectoryName(typeof(Program).Assembly.Location) : args[0]);
            data = CauldronBuilderData.GetConfig(startingLocation);

            var version = data.IsBeta ? data.CurrentPackageVersion + "-beta" : data.CurrentPackageVersion;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Version {version} / {data.CurrentAssemblyVersion}");
            Console.WriteLine($"Path {startingLocation.FullName}");
            Console.ResetColor();

            try
            {
                var packages = new DirectoryInfo(Path.Combine(startingLocation.FullName, "Nuget\\Packages"));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Do you want to build the projects? Y/N");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("");

                    var allProjectFiles = Directory.GetFiles(startingLocation.FullName, "*.csproj", SearchOption.AllDirectories)
                        .Select(x => new FileInfo(x))
                        .Where(x => !x.Name.Contains(".Test") && x.Name.StartsWith("Cauldron") && !x.FullName.Contains("\\Old\\"))
                        .Select(x => new { Index = x.Name.Contains("Fody") ? 0 : 1, File = x })
                        .OrderBy(x => x.Index)
                        .Select(x => x.File)
                        .ToArray();
                    var solutionPath = new FileInfo(Path.Combine(startingLocation.FullName, "Cauldron.sln"));

                    foreach (var project in allProjectFiles)
                    {
                        ModifyAssemblyInfo(project);
                        ChangeVersion(project, version);
                    }

                    for (int i = 0; i < allProjectFiles.Length; i++)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Compiling [{i + 1}/{allProjectFiles.Length}] " + allProjectFiles[i].Name);
                        BuildProject(solutionPath, allProjectFiles[i]);
                    }

                    // Clean the package directory
                    foreach (var file in Directory.GetFiles(packages.FullName, "*.nupkg"))
                        File.Delete(file);

                    // Move all packages to the Package directory
                    foreach (var nuget in Directory.GetFiles(startingLocation.FullName, "*.nupkg", SearchOption.AllDirectories)
                        .Where(x => x.Contains("\\Release\\")))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Moving " + Path.GetFileName(nuget));
                        File.Move(nuget, Path.Combine(startingLocation.FullName, "Nuget\\Packages", Path.GetFileName(nuget)));
                    }

                    data.IncrementAndSave();
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Do you want to build the Nuget packages? Y/N");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    // Build nugets of non NetStandard2.0 projects
                    Parallel.ForEach(Directory.GetFiles(Path.Combine(startingLocation.FullName, "Nuget"), "*.nuspec").Select(x => new FileInfo(x)), nuget =>
                       {
                           ModifyNuspec(nuget, version);
                           BuildNuGetPackage(nuget.FullName, packages.FullName, version);
                       });
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Do you want to upload the packages? Y/N");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("");

                    foreach (var package in Directory.GetFiles(packages.FullName, "*.nupkg", SearchOption.TopDirectoryOnly)
                        .Where(x => x.IndexOf(".symbols.", StringComparison.InvariantCultureIgnoreCase) < 0))
                        UploadNugetPackage(package);
                }

                CreateReadmeFromNuspec(startingLocation, version);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.GetStackTrace());
                Console.WriteLine("\r\n\r\nCancelled");
                Console.Read();
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private static void BuildNuGetPackage(string path, string targetDirectory, string version)
        {
            var filename = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "nuget.exe"));

            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = Path.GetDirectoryName(path);
            startInfo.FileName = filename.FullName;
            startInfo.Arguments = string.Format("pack \"{0}\" -Symbols -OutputDir Packages -version {2}", path, targetDirectory, version);
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Compiling " + startInfo.Arguments);

            var process = Process.Start(startInfo);
            var error = process.StandardError.ReadToEnd();
            var output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            Console.WriteLine(output);

            if (process.ExitCode != 0)
                throw new Exception(error);
        }

        private static void BuildProject(FileInfo solutionPath, FileInfo path)
        {
            var filename = new FileInfo(@"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe");

            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = path.DirectoryName;
            startInfo.FileName = filename.FullName;
            startInfo.Arguments = string.Format("\"{1}\" /target:Clean;Rebuild /p:Configuration=Release", solutionPath.FullName, path.FullName);
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;

            var process = Process.Start(startInfo);
            var error = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception(error);
        }

        private static void ChangeVersion(FileInfo path, string version)
        {
            var projectFileBody = File.ReadAllText(path.FullName);
            if (projectFileBody.StartsWith(@"<Project Sdk=""Microsoft.NET.Sdk"">")) // This is a NetStandard project
            {
                var packageVersion = projectFileBody.EnclosedIn("<Version>", "</Version>");
                if (packageVersion != null)
                    projectFileBody = projectFileBody.Replace(packageVersion, $"<Version>{version}</Version>");

                var assemblyVersion = projectFileBody.EnclosedIn("<AssemblyVersion>", "</AssemblyVersion>");
                if (assemblyVersion != null)
                    projectFileBody = projectFileBody.Replace(assemblyVersion, $"<AssemblyVersion>{data.CurrentAssemblyVersion}</AssemblyVersion>");
                else
                    Insert(ref projectFileBody, $"<AssemblyVersion>{data.CurrentAssemblyVersion}</AssemblyVersion>\r\n");

                var fileVersion = projectFileBody.EnclosedIn("<FileVersion>", "</FileVersion>");
                if (fileVersion != null)
                    projectFileBody = projectFileBody.Replace(fileVersion, $"<FileVersion>{data.CurrentAssemblyVersion}</FileVersion>");
                else
                    Insert(ref projectFileBody, $"<FileVersion>{data.CurrentAssemblyVersion}</FileVersion>\r\n");

                File.WriteAllText(path.FullName, projectFileBody);
            }
            else
            {
                // Lets look for AssemblyInfo.cs
                var assemblyInfo = Path.Combine(path.Directory.FullName, "Properties\\AssemblyInfo.cs");
                if (!File.Exists(assemblyInfo))
                    return;

                var assemblyInfoBody = File.ReadAllText(assemblyInfo);

                var assemblyVersion = assemblyInfoBody.EnclosedIn("[assembly: AssemblyVersion(\"", "\")]");
                if (assemblyVersion != null)
                    assemblyInfoBody = assemblyInfoBody.Replace(assemblyVersion, $"[assembly: AssemblyVersion(\"{data.CurrentAssemblyVersion}\")]");

                var fileVersion = assemblyInfoBody.EnclosedIn("[assembly: AssemblyFileVersion(\"", "\")]");
                if (fileVersion != null)
                    assemblyInfoBody = assemblyInfoBody.Replace(fileVersion, $"[assembly: AssemblyFileVersion(\"{data.CurrentAssemblyVersion}\")]");

                File.WriteAllText(assemblyInfo, assemblyInfoBody);
            }
        }

        private static bool Contains(this XmlElement element, string key)
        {
            try
            {
                var result = element[key];
                return result != null;
            }
            catch
            {
                return false;
            }
        }

        private static void CreateReadmeFromNuspec(DirectoryInfo startingLocation, string currentVersion)
        {
            var nugetbag = new ConcurrentBag<string>();
            var historybag = new ConcurrentBag<NuspecInfo>();

            Parallel.ForEach(Directory.GetFiles(Path.Combine(startingLocation.FullName, "Nuget"), "*.nuspec").Select(x => new FileInfo(x)), nuget =>
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Getting Information for " + nuget);

                var nuspec = new XmlDocument();
                nuspec.Load(nuget.FullName);

                var name = nuspec["package"]["metadata"]["id"].InnerText;
                var description = nuspec["package"]["metadata"]["description"].InnerText?.Replace("\r\n", "<br/>");
                var nugetLink = nuspec["package"]["metadata"]["id"].InnerText;

                if (name.StartsWith("Capgemini."))
                    name = name.Substring("Capgemini.".Length);

                if (description.StartsWith("<br/>"))
                    description = description.Substring("<br/>".Length).Trim();

                nugetLink = $"[![NuGet](https://img.shields.io/nuget/v/{nugetLink}.svg)](https://www.nuget.org/packages/{nugetLink}/)";

                nugetbag.Add($"**{name}** | {description} | {nugetLink}");

                var nugetInfos = GetNugetInfo(Path.GetFileNameWithoutExtension(nuget.Name)).Result;
                var nugetInfo = nugetInfos.Items
                    .SelectMany(x => x.Items)
                    .Select(x => x.CatalogEntry)
                    .Concat(new CatalogEntry[] { new CatalogEntry { Version = currentVersion, Published = DateTime.Now } })
                    .OrderBy(x => x.Published)
                    .ToArray();
                var versionInfo = nuspec["package"]["metadata"]["releaseNotes"]?.InnerText?.Split("\r\n")
                     .Select(x =>
                     {
                         if (string.IsNullOrEmpty(x))
                             return null;

                         var cleanLine = x.Trim();

                         if (string.IsNullOrEmpty(cleanLine))
                             return null;

                         var itemDate = DateTime.TryParse(cleanLine.Substring(0, 10), out DateTime date) ? date : (DateTime?)null;
                         var metaData = nugetInfo.FirstOrDefault(y => y.Published >= itemDate);
                         return new NuspecInfo
                         {
                             Date = itemDate ?? DateTime.Now,
                             Type = cleanLine.EnclosedIn("[", "]"),
                             Description = $"__{Path.GetFileNameWithoutExtension(nuget.Name)}:__ _{cleanLine.Substring(cleanLine.IndexOf(']') + 1)?.Trim() ?? ""}_",
                             Version = metaData.Version
                         };
                     });

                if (versionInfo == null)
                    return;

                foreach (var item in versionInfo)
                    historybag.Add(item);
            });

            var versionHistory = historybag
                .Where(x => x != null)
                .GroupBy(x => x.Version)
                .Select(x => new { Version = x.Key, Types = x.GroupBy(y => y.Type).Select(y => new { Type = y.Key, Log = y.ToArray() }) })
                .OrderByDescending(x => x.Version)
                .ThenBy(x => x.Types)
                .ToArray();

            var historyNugetInfo = new List<string>();
            foreach (var version in versionHistory)
            {
                historyNugetInfo.Add($"### __{version.Version}__");

                foreach (var type in version.Types)
                {
                    switch (type.Type)
                    {
                        case "[A]":
                            historyNugetInfo.Add("#### Added");
                            break;

                        case "[B]":
                            historyNugetInfo.Add("#### Bugfix");
                            break;

                        case "[C]":
                            historyNugetInfo.Add("#### Change");
                            break;
                    }

                    foreach (var text in type.Log)
                        historyNugetInfo.Add($"- {text.Description}");
                }
            }

            var template = File.ReadAllText(Path.Combine(startingLocation.FullName, "Nuget", "Readme-template-.md"));
            var nugetPackages = string.Join("\r\n", nugetbag.OrderBy(x => x));

            File.WriteAllText(Path.Combine(startingLocation.FullName, "README.md"),
                template
                    .Replace("<NUGET_PACKAGES>", nugetPackages)
                    .Replace("<RELEASE_NOTES>", string.Join("\r\n", historyNugetInfo)));
        }

        private static string EnclosedIn(this string target, string start, string end)
        {
            if (string.IsNullOrEmpty(target))
                return null;

            int startPos = target.IndexOf(start);

            if (startPos < 0)
                return null;

            int endPos = target.IndexOf(end, startPos + 1);

            if (endPos <= startPos)
                return null;

            return target.Substring(startPos, endPos - startPos + end.Length);
        }

        private static async Task<CatalogRoot> GetNugetInfo(string packageName)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"https://api.nuget.org/v3/registration3/{packageName.ToLower()}/index.json");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CatalogRoot>(data);
                }
            }

            return null;
        }

        private static string GetStackTrace(this Exception e)
        {
            var sb = new StringBuilder();
            var ex = e;

            do
            {
                sb.AppendLine("Exception Type: " + ex.GetType().Name);
                sb.AppendLine("Source: " + ex.Source);
                sb.AppendLine(ex.Message);
                sb.AppendLine("------------------------");
                sb.AppendLine(ex.StackTrace);
                sb.AppendLine("------------------------");

                ex = ex.InnerException;
            } while (ex != null);

            return sb.ToString();
        }

        private static void Insert(ref string body, string data)
        {
            int position = body.IndexOf("</PropertyGroup>");

            if (position < 0)
                return;

            body = body.Insert(position, data);
        }

        private static void ModifyAssemblyInfo(FileInfo path)
        {
            var projectFileBody = File.ReadAllText(path.FullName);
            if (projectFileBody.StartsWith(@"<Project Sdk=""Microsoft.NET.Sdk"">")) // This is a NetStandard project
            {
                var authors = projectFileBody.EnclosedIn("<Authors>", "</Authors>");
                if (authors != null)
                    projectFileBody = projectFileBody.Replace(authors, $"<Authors>{Authors}</Authors>");
                else
                    Insert(ref projectFileBody, $"<Authors>{Authors}</Authors>\r\n");

                var copyright = projectFileBody.EnclosedIn("<Copyright>", "</Copyright>");
                if (copyright != null)
                    projectFileBody = projectFileBody.Replace(copyright, $"<Copyright>{Copyright}</Copyright>");
                else
                    Insert(ref projectFileBody, $"<Copyright>{Copyright}</Copyright>\r\n");

                File.WriteAllText(path.FullName, projectFileBody);
            }
            else
            {
                // Lets look for AssemblyInfo.cs
                var assemblyInfo = Path.Combine(path.Directory.FullName, "Properties\\AssemblyInfo.cs");
                if (!File.Exists(assemblyInfo))
                    return;

                var assemblyInfoBody = File.ReadAllText(assemblyInfo);

                var authors = assemblyInfoBody.EnclosedIn("[assembly: AssemblyCompany(\"", "\")]");
                if (authors != null)
                    assemblyInfoBody = assemblyInfoBody.Replace(authors, $"[assembly: AssemblyCompany(\"{Authors}\")]");

                var copyright = assemblyInfoBody.EnclosedIn("[assembly: AssemblyCopyright(\"", "\")]");
                if (copyright != null)
                    assemblyInfoBody = assemblyInfoBody.Replace(copyright, $"[assembly: AssemblyCopyright(\"{Copyright}\")]");

                File.WriteAllText(assemblyInfo, assemblyInfoBody);
            }
        }

        private static void ModifyNuspec(FileInfo path, string version)
        {
            var nuspec = new XmlDocument();
            nuspec.Load(path.FullName);
            var dependencies = nuspec["package"]["metadata"]["dependencies"];

            foreach (XmlElement item in dependencies.Contains("group") ? dependencies["group"] : dependencies)
            {
                if (item.Attributes["id"].Value.Contains("Cauldron"))
                    item.Attributes["version"].Value = version;
            }

            nuspec["package"]["metadata"]["owners"].InnerText = Authors;
            nuspec["package"]["metadata"]["authors"].InnerText = Authors;
            nuspec["package"]["metadata"]["requireLicenseAcceptance"].InnerText = "true";
            nuspec["package"]["metadata"]["licenseUrl"].InnerText = "https://raw.githubusercontent.com/Capgemini/Cauldron/master/LICENSE";
            nuspec["package"]["metadata"]["projectUrl"].InnerText = "https://github.com/Capgemini/Cauldron";
            nuspec["package"]["metadata"]["iconUrl"].InnerText = "https://raw.githubusercontent.com/Capgemini/Cauldron/master/cauldron.png";
            nuspec["package"]["metadata"]["copyright"].InnerText = Copyright;
            nuspec["package"]["metadata"]["id"].InnerText = Path.GetFileNameWithoutExtension(path.FullName);

            nuspec.Save(path.FullName);
        }

        private static void UploadNugetPackage(string path)
        {
            var filename = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "nuget.exe"));

            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = Path.GetDirectoryName(path);
            startInfo.FileName = filename.FullName;
            startInfo.Arguments = string.Format("push \"{0}\" -Source https://www.nuget.org/api/v2/package", path);
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Uploading " + startInfo.Arguments);
            Console.ResetColor();

            var process = Process.Start(startInfo);
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            process.WaitForExit();
            Console.WriteLine(output);

            if (process.ExitCode != 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ResetColor();
            }
        }
    }
}