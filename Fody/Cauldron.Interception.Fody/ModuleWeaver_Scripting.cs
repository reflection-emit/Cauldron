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
        public void ExecuteInterceptionScripts(Builder builder)
        {
            var csc = Path.Combine(this.AddinDirectoryPath, "csc\\csc.exe");

            using (new StopwatchLog(this, "custom interceptors"))
            {
                var scriptBinaries = new List<Assembly>();
                var interceptorDirectory = Path.Combine(this.ProjectDirectoryPath, "Interceptors");
                var dlls = Directory.GetFiles(this.AddinDirectoryPath, "*.dll");

                if (!Directory.Exists(interceptorDirectory))
                    return;

                // Find all uncompiled scripts and compile them
                foreach (var script in Directory.GetFiles(interceptorDirectory, "*.csx", SearchOption.AllDirectories))
                    scriptBinaries.Add(AppDomain.CurrentDomain.Load(File.ReadAllBytes(CompileScript(csc, script, dlls))));

                foreach (var script in Directory.GetFiles(interceptorDirectory, "*.dll", SearchOption.AllDirectories))
                    scriptBinaries.Add(AppDomain.CurrentDomain.Load(File.ReadAllBytes(script)));

                foreach (var scriptBinary in scriptBinaries
                            .SelectMany(x => x.DefinedTypes)
                            .Where(x => x.GetMethods(BindingFlags.Public | BindingFlags.Static).Length > 0)
                            .Select(x => new
                            {
                                Type = x,
                                Priority = (int)(x.GetField("Priority", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ?? int.MaxValue),
                                Name = x.GetField("Name", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as string ?? x.Name,
                                Implement = x.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                    .Where(y => y.GetParameters().Length == 1 && y.GetParameters()[0].ParameterType == typeof(Builder))
                                    .OrderBy(y => y.Name)
                                    .ToArray()
                            })
                            .OrderBy(x => x.Priority))
                {
                    using (new StopwatchLog(this, scriptBinary.Name))
                    {
                        this.Log(LogTypes.Info, "++++++ Executing custom interceptor: " + scriptBinary.Name + " ++++++");

                        foreach (var method in scriptBinary.Implement)
                        {
                            this.Log(LogTypes.Info, "-----> Executing method: " + method.Name);
                            method.Invoke(null, new object[] { builder });
                        }
                    }
                }
            }
        }

        private string CompileScript(string compiler, string script, string[] references)
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
    }
}