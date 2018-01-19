using Cauldron.Interception.Cecilator;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        public void ExecuteInterceptionScripts(Builder builder)
        {
            using (new StopwatchLog(this, "scripted"))
            {
                foreach (var script in
                    Directory.GetFiles(this.ProjectDirectoryPath, "*.cauldron", SearchOption.TopDirectoryOnly).Concat(
                        Directory.Exists(Path.Combine(this.ProjectDirectoryPath, "Interceptors")) ?
                        Directory.GetFiles(Path.Combine(this.ProjectDirectoryPath, "Interceptors"), "*.cauldron", SearchOption.TopDirectoryOnly) : new string[0])
                        .Select(x => AppDomain.CurrentDomain.Load(File.ReadAllBytes(x)))
                        .Select(x => x.GetType("Interceptor"))
                        .Select(x => new
                        {
                            Type = x,
                            Priority = (int)(x.GetField("Priority", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ?? 0),
                            Name = x.GetField("Name", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as string ?? x.Name,
                            Implement = x.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                .FirstOrDefault(y => y.GetParameters().Length == 1 && y.GetParameters()[0].ParameterType == typeof(Builder))
                        })
                        .OrderBy(x => x.Priority))
                {
                    using (new StopwatchLog(this, script.Name))
                    {
                        this.Log(LogTypes.Info, "++++++ Executing script: " + script.Name + " ++++++");
                        script.Implement.Invoke(null, new object[] { builder });
                    }
                }
            }
        }
    }
}