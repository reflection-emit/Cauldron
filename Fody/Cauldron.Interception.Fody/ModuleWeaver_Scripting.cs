using Cauldron.Interception.Cecilator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        public List<string> CustomInterceptors { get; } = new List<string>();

        public void ExecuteInterceptionScripts(Builder builder)
        {
            var csc = Path.Combine(this.AddinDirectoryPath, "csc\\csc.exe");
            this.GetCustomInterceptorList();

            using (new StopwatchLog(this, "custom interceptors"))
            {
                var scriptBinaries = new List<Assembly>();
                var interceptorDirectory = Path.Combine(this.ProjectDirectoryPath, "Interceptors");
                var dlls = new List<string>();

                dlls.AddRange(Directory.GetFiles(this.AddinDirectoryPath, "*.dll"));
                dlls.AddRange(typeof(ModuleWeaver).Assembly.GetReferencedAssemblies().Select(x => Assembly.Load(x).Location?.Trim()).Where(x => !string.IsNullOrEmpty(x)));

                if (Directory.Exists(interceptorDirectory))
                {
                    // Find all uncompiled scripts and compile them
                    foreach (var script in Directory.GetFiles(interceptorDirectory, "*.csx", SearchOption.AllDirectories))
                        scriptBinaries.Add(AppDomain.CurrentDomain.Load(File.ReadAllBytes(CompileScript(csc, script, dlls))));

                    foreach (var script in Directory.GetFiles(interceptorDirectory, "*.dll", SearchOption.AllDirectories))
                        scriptBinaries.Add(AppDomain.CurrentDomain.Load(File.ReadAllBytes(script)));
                }

                scriptBinaries.AddRange(this.CustomInterceptors.Select(x => AppDomain.CurrentDomain.Load(File.ReadAllBytes(x))));

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

        private string CompileScript(string compiler, string script, IEnumerable<string> references)
        {
            this.Log(LogTypes.Info, "       Compiling custom interceptor: " + Path.GetFileName(script));

            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(script));
            var output = Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(script) + ".dll");

            Directory.CreateDirectory(tempDirectory);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = compiler,
                Arguments = $"/t:library /out:\"{output}\" /optimize+  \"{script}\" /r:{string.Join(",", references.Select(x => "\"" + x + "\""))}",
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
                throw new Exception($"An error has occured while compiling '{script}'");

            return output;
        }

        private void GetCustomInterceptorList()
        {
            var element = this.Config.Element(nameof(CustomInterceptors));

            if (element == null)
                return;

            foreach (var item in element.Value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var path = item
                    .Trim()
                    .Replace("$(SolutionPath)", this.SolutionDirectoryPath)
                    .Replace("$(ProjectDir)", this.ProjectDirectoryPath);

                if (string.IsNullOrEmpty(path))
                    continue;

                if (!File.Exists(path))
                    throw new FileNotFoundException("File not found: " + path);

                this.CustomInterceptors.Add(path);
            }
        }
    }
}