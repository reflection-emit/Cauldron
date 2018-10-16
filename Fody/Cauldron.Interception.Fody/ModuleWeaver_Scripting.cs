using Cauldron.Interception.Cecilator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private string cscPath;

        private List<string> referencedDlls = new List<string>();

        static ModuleWeaver()
        {
        }

        public void ExecuteInterceptionScripts(Builder builder)
        {
            this.GetCustomInterceptorList();

            using (new StopwatchLog(this, "custom interceptors"))
            {
                var scriptBinaries = new List<Assembly>();
                var interceptorDirectory = Path.Combine(this.ProjectDirectoryPath, "Interceptors");
                var scripts = this.GetCustomInterceptorList();

                this.cscPath = Path.Combine(this.AddinDirectoryPath, "csc\\csc.exe");
                this.referencedDlls.AddRange(Directory.GetFiles(this.AddinDirectoryPath, "*.dll"));
                this.referencedDlls.AddRange(typeof(ModuleWeaver).Assembly.GetReferencedAssemblies().Select(x => Assembly.Load(x).Location?.Trim()).Where(x => !string.IsNullOrEmpty(x)));

                if (Directory.Exists(interceptorDirectory))
                    scripts = scripts
                        .Concat(Directory.GetFiles(interceptorDirectory, "*.csx", SearchOption.AllDirectories))
                        .Concat(Directory.GetFiles(interceptorDirectory, "*.dll", SearchOption.AllDirectories));

                scripts = scripts
                    .Concat(this.GetNugetPropsInterceptorPaths())
                    .Concat(this.GetNugetJsonInterceptorPaths())
                    .Distinct(new ScriptNameEqualityComparer());
                scriptBinaries.AddRange(scripts.Select(x => this.LoadScript(x)));

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
                                    .Where(y => y.Parameters.Length == 1 && y.Parameters[0].ParameterType.FullName == typeof(Builder).FullName)
                                    .OrderBy(y => y.MethodInfo.Name)
                                    .ToArray()
                            })
                            .OrderBy(x => x.Priority))
                {
                    var speed = new StopwatchLog(this, scriptBinary.Name);

                    try
                    {
                        this.Log(LogTypes.Info, ">> Executing custom interceptors in: " + scriptBinary.Name);
                        var config = scriptBinary.Type.GetProperty("Config", BindingFlags.Static | BindingFlags.Public);
                        var projectDirectoryPath = scriptBinary.Type.GetProperty("ProjectDirectoryPath", BindingFlags.Static | BindingFlags.Public);
                        var solutionDirectoryPath = scriptBinary.Type.GetProperty("SolutionDirectoryPath", BindingFlags.Static | BindingFlags.Public);

                        if (config != null) config.SetValue(null, this.Config);
                        if (projectDirectoryPath != null) projectDirectoryPath.SetValue(null, this.ProjectDirectoryPath);
                        if (solutionDirectoryPath != null) solutionDirectoryPath.SetValue(null, this.SolutionDirectoryPath);

                        foreach (var method in scriptBinary.Implement)
                        {
                            var display = method.MethodInfo.GetCustomAttributes().FirstOrDefault(x => x.GetType().FullName == typeof(DisplayAttribute).FullName);
                            var name = display?.GetType()?.GetProperty("Name")?.GetValue(display) as string ?? method.MethodInfo.Name;

                            this.Log(LogTypes.Info, "   Executing custom interceptor: " + name);
                            method.MethodInfo.Invoke(null, new object[] { builder });
                        }
                    }
                    catch (ArgumentException e)
                    {
                        throw new Exception("Unable to execute the script. Possible wrong version.", e);
                    }
                    catch (InvalidCastException e)
                    {
                        throw new Exception("Unable to execute the script. Possible wrong version.", e);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        speed.Dispose();
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

            Directory.CreateDirectory(tempDirectory);

            var additionalReferences = this.GetReferences(path, tempDirectory);
            if (additionalReferences.Item3 && File.Exists(output))
                return output;

            var arguments = new string[]
            {
                $"/t:library",
                $"/out:\"{output}\"",
                $"/optimize+",
                $"\"{additionalReferences.Item2}\"",
                $"/pdb:\"{pdb}\"",
                $"/r:{string.Join(",", references.Concat(additionalReferences.Item1).Select(x => "\"" + x + "\""))}"
            };

            if (this.ExecuteExternalApplication(compiler, arguments, tempDirectory) != 0)
                throw new Exception($"An error has occured while compiling '{additionalReferences.Item2}'");

            return output;
        }

        private IEnumerable<string> GetCustomInterceptorList()
        {
            var element = this.Config.Element("CustomInterceptors");

            if (element == null)
                yield break;

            foreach (var item in element.Value.Split(new[] { "\r\n", "\n", ", ", " " }, StringSplitOptions.RemoveEmptyEntries))
            {
                var path = this.GetFullPath(item);

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

        private Tuple<IEnumerable<string>, string, bool> GetReferences(string path, string tempDirectory)
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

                var data = string.Join("\r\n", lines.Where(x =>
                {
                    if (x == null)
                        return false;

                    if (x.Trim().StartsWith("#r "))
                        return false;

                    return true;
                }).ToArray());

                var hash = UTF8Encoding.UTF8.GetBytes(data).GetMd5Hash();
                var output = Path.Combine(tempDirectory, hash + ".cs");

                if (File.Exists(output))
                    return new Tuple<IEnumerable<string>, string, bool>(references, output, true);

                foreach (var item in Directory.GetFiles(tempDirectory))
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch
                    {
                    }
                }

                File.WriteAllText(output, data);
                return new Tuple<IEnumerable<string>, string, bool>(references, output, false);
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
                case ".CSX": return AppDomain.CurrentDomain.Load(File.ReadAllBytes(this.CompileScript(this.cscPath, path, this.referencedDlls)));
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