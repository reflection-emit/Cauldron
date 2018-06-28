using Cauldron.Interception.Cecilator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private string cscPath;
        private List<string> referencedDlls = new List<string>();

        public void ExecuteInterceptionScripts(Builder builder)
        {
            this.GetCustomInterceptorList();

            using (new StopwatchLog(this, "custom interceptors"))
            {
                var scriptBinaries = new List<Assembly>();
                var interceptorDirectory = Path.Combine(this.ProjectDirectoryPath, "Interceptors");
                var scripts = GetCustomInterceptorList();

                cscPath = Path.Combine(this.AddinDirectoryPath, "csc\\csc.exe");
                referencedDlls.AddRange(Directory.GetFiles(this.AddinDirectoryPath, "*.dll"));
                referencedDlls.AddRange(typeof(ModuleWeaver).Assembly.GetReferencedAssemblies().Select(x => Assembly.Load(x).Location?.Trim()).Where(x => !string.IsNullOrEmpty(x)));

                if (Directory.Exists(interceptorDirectory))
                    scripts = scripts
                        .Concat(Directory.GetFiles(interceptorDirectory, "*.csx", SearchOption.AllDirectories))
                        .Concat(Directory.GetFiles(interceptorDirectory, "*.dll", SearchOption.AllDirectories));

                scripts = scripts
                    .Concat(GetNugetPropsInterceptorPaths())
                    .Concat(GetNugetJsonInterceptorPaths())
                    .Distinct(new ScriptNameEqualityComparer());
                scriptBinaries.AddRange(scripts.Select(x => LoadScript(x)));

                builder.Log(LogTypes.Info, "Found Custom interceptors");
                foreach (var interceptorPath in scripts)
                    builder.Log(LogTypes.Info, "- " + interceptorPath);

                foreach (var scriptBinary in scriptBinaries
                            .SelectMany(x => x.DefinedTypes)
                            .Where(x => x.GetMethods(BindingFlags.Public | BindingFlags.Static).Length > 0)
                            .Select(x => new
                            {
                                Type = x,
                                Priority = (int)(x.GetField("Priority", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ?? int.MaxValue),
                                Name = x.GetField("Name", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as string ?? x.Name,
                                Implement = x.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                    .Select(y => new
                                    {
                                        MethodInfo = y,
                                        Parameters = y.GetParameters()
                                    })
                                    .Where(y => y.Parameters.Length == 1 && y.Parameters[0].ParameterType == typeof(Builder))
                                    .OrderBy(y => y.MethodInfo.Name)
                                    .ToArray()
                            })
                            .OrderBy(x => x.Priority))
                {
                    using (new StopwatchLog(this, scriptBinary.Name))
                    {
                        this.Log(LogTypes.Info, ">> Executing custom interceptors in: " + scriptBinary.Name);
                        var config = scriptBinary.Type.GetProperty("Config", BindingFlags.Static | BindingFlags.Public);

                        if (config != null)
                            config.SetValue(null, this.Config);

                        foreach (var method in scriptBinary.Implement)
                        {
                            var display = method.MethodInfo.GetCustomAttributes().FirstOrDefault(x => x.GetType().FullName == typeof(DisplayAttribute).FullName);
                            var name = display?.GetType()?.GetProperty("Name")?.GetValue(display) as string ?? method.MethodInfo.Name;

                            this.Log(LogTypes.Info, "   Executing custom interceptor: " + name);
                            method.MethodInfo.Invoke(null, new object[] { builder });
                        }
                    }
                }
            }
        }

        private string CompileScript(string compiler, string path, IEnumerable<string> references)
        {
            this.Log(LogTypes.Info, "       Compiling custom interceptor: " + Path.GetFileName(path));

            var tempDirectory = Path.Combine(Path.GetTempPath(), this.ProjectName, Path.GetFileNameWithoutExtension(path) + "-" + typeof(ModuleWeaver).Assembly.GetName().Version);
            var output = Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(path) + ".dll");
            var pdb = Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(path) + ".pdb");

            if (File.Exists(output))
                return output;

            Directory.CreateDirectory(tempDirectory);

            var additionalReferences = GetReferences(path, tempDirectory);
            var arguments = new string[]
            {
                $"/t:library",
                $"/out:\"{output}\"",
                $"/optimize+",
                $"\"{additionalReferences.Item2}\"",
                $"/pdb:\"{pdb}\"",
                $"/r:{string.Join(",", references.Concat(additionalReferences.Item1).Select(x => "\"" + x + "\""))}"
            };

            var processStartInfo = new ProcessStartInfo
            {
                FileName = compiler,
                Arguments = string.Join(" ", arguments),
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = tempDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = new Process
            {
                StartInfo = processStartInfo
            };

            process.OutputDataReceived += (s, e) => Builder.Current.Log(LogTypes.Info, e.Data);
            process.ErrorDataReceived += (s, e) => Builder.Current.Log(LogTypes.Info, e.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception($"An error has occured while compiling '{additionalReferences.Item2}'");

            return output;
        }

        private IEnumerable<string> GetCustomInterceptorList()
        {
            var element = this.Config.Element("CustomInterceptors");

            if (element == null)
                yield break;

            foreach (var item in element.Value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var path = item
                    .Trim()
                    .Replace("$(SolutionPath)", this.SolutionDirectoryPath)
                    .Replace("$(ProjectDir)", this.ProjectDirectoryPath);

                if (string.IsNullOrEmpty(path))
                    continue;

                var dlls = Directory.GetFiles(Path.GetDirectoryName(path), Path.GetFileName(path));

                if (dlls == null || dlls.Length == 0)
                    throw new FileNotFoundException("File not found: " + path);

                for (int i = 0; i < dlls.Length; i++)
                    yield return dlls[i];
            }
        }

        private IEnumerable<string> GetNugetJsonInterceptorPaths()
        {
            var projectObjPath = Path.Combine(this.ProjectDirectoryPath, "obj");
            if (!Directory.Exists(projectObjPath))
                yield break;

            var nugetFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget\\packages\\");
            var jsonFile = Directory.GetFiles(projectObjPath, "project.assets.json", SearchOption.TopDirectoryOnly).FirstOrDefault();

            if (jsonFile == null)
                yield break;

            var jObject = JsonConvert.DeserializeObject(File.ReadAllText(jsonFile)) as JObject;

            foreach (var item in jObject
                .GetChildren<JProperty>(x => x.Name == "targets")
                .SelectMany(x => x.GetChildren())
                .SelectMany(x => x.GetChildren())
                .SelectMany(x => x.GetChildren())
                .SelectMany(x => x.GetChildren())
                .SelectMany(x => x.GetChildren())
                .SelectMany(x => x.GetChildren<JProperty>(y => y.Name == "contentFiles"))
                .SelectMany(x => x.GetChildren())
                .SelectMany(x => x.GetChildren<JProperty>(y => y.Name.IndexOf(@"contentFiles/any/any/Interceptors/", StringComparison.InvariantCultureIgnoreCase) >= 0))
                .Select(x => Path.Combine(nugetFolder, (x.Parent.Parent.Parent.Parent as JProperty).Name, (x as JProperty).Name).Replace('/', '\\'))
                .Distinct())
                yield return item;
        }

        private IEnumerable<string> GetNugetPropsInterceptorPaths()
        {
            var projectObjPath = Path.Combine(this.ProjectDirectoryPath, "obj");
            if (!Directory.Exists(projectObjPath))
                yield break;

            var nugetFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget\\packages\\");
            var path = Directory.GetFiles(projectObjPath, "*.csproj.nuget.g.props", SearchOption.TopDirectoryOnly).FirstOrDefault();

            if (path == null)
                yield break;

            var xml = new XmlDocument();
            xml.Load(path);

            foreach (XmlNode item in xml.LastChild.ChildNodes)
            {
                if (item.Name != "ItemGroup")
                    continue;

                foreach (XmlNode itemGroup in item.ChildNodes)
                {
                    var value = itemGroup.Attributes["Include"]?.Value;
                    if (!string.IsNullOrEmpty(value) && value.IndexOf(@"\contentFiles\any\any\Interceptors\", StringComparison.InvariantCultureIgnoreCase) >= 0)
                        yield return value.Replace("$(NuGetPackageRoot)", nugetFolder);
                }
            }
        }

        private Tuple<IEnumerable<string>, string> GetReferences(string path, string tempDirectory)
        {
            var assemblyDomain = AppDomain.CreateDomain(friendlyName: "spider-man");

            try
            {
                var references = new List<string>();
                var lines = File.ReadAllLines(path);

                foreach (var @ref in lines.Where(x => x?.Trim().StartsWith("#r ") ?? false))
                {
                    try
                    {
                        var assembly = assemblyDomain.Load(@ref.EnclosedIn("\"", "\""));
                        references.Add(assembly.Location);
                    }
                    catch
                    {
                    }
                }

                var output = Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(path) + ".cs");

                File.WriteAllLines(output, lines.Where(x =>
                {
                    if (x == null)
                        return false;

                    if (x.Trim().StartsWith("#r "))
                        return false;

                    return true;
                }).ToArray());

                return new Tuple<IEnumerable<string>, string>(references, output);
            }
            finally
            {
                AppDomain.Unload(assemblyDomain);
            }
        }

        private Assembly LoadScript(string path)
        {
            switch (Path.GetExtension(path))
            {
                case ".csx":
                case ".CSX": return AppDomain.CurrentDomain.Load(File.ReadAllBytes(CompileScript(cscPath, path, referencedDlls)));
                case ".dll":
                case ".DLL": return AppDomain.CurrentDomain.Load(File.ReadAllBytes(path));
            }

            throw new NotSupportedException("Unsupported file type");
        }

        private class ScriptNameEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y) =>
                string.Equals(Path.GetFileName(x), Path.GetFileName(y), StringComparison.InvariantCultureIgnoreCase) &&
                    new FileInfo(x).Length == new FileInfo(y).Length;

            public int GetHashCode(string obj) => Path.GetFileName(obj).ToLower().GetHashCode();
        }
    }
}