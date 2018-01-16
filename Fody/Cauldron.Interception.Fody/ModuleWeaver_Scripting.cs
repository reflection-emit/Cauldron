using Cauldron.Interception.Cecilator;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.IO;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        public void ExecuteInterceptionScripts(Builder builder)
        {
            using (new StopwatchLog(this, "scripts"))
            {
                foreach (var script in Directory.GetFiles(this.ProjectDirectoryPath, "*cauldron.fody.csx", SearchOption.TopDirectoryOnly))
                    this.Log(LogTypes.Info, CSharpScript.Create(
                        File.ReadAllText(script),
                        ScriptOptions.Default.AddImports(new string[] {
                            "System",
                            "System.IO",
                            "Cauldron.Interception.Cecilator",
                            "Cauldron.Interception.Fody",
                            "Mono.Cecil",
                            "System.Linq"
                        }), globalsType: typeof(Globals))
                        .RunAsync(new Globals { Builder = builder })
                        .Result);
            }
        }
    }
}