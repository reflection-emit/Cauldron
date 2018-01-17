using Cauldron.Interception.Cecilator;
using CSScriptLibrary;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Cauldron.Interception.Fody
{
    public interface IInterceptionScript
    {
        string Name { get; }
        int Priority { get; }

        void Implement(Builder builder);
    }

    public sealed partial class ModuleWeaver
    {
        public void ExecuteInterceptionScripts(Builder builder)
        {
            string Unzip(string path)
            {
                var target = new MemoryStream();

                using (var stream = File.OpenRead(path))
                using (var decompressionStream = new GZipStream(stream, CompressionMode.Decompress, true))
                    decompressionStream.CopyTo(target);

                target.Seek(0, SeekOrigin.Begin);
                return Encoding.UTF8.GetString(target.ToArray());
            }

            using (new StopwatchLog(this, "scripted"))
            {
                CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Mono;

                var evaluator = CSScript.Evaluator;
                evaluator.Reset(false);
                evaluator.ReferenceDomainAssemblies()
                    .ReferenceAssemblyOf<ModuleWeaver>()
                    .ReferenceAssemblyOf<CecilatorObject>();

                foreach (var script in Directory.GetFiles(this.ProjectDirectoryPath, "*cauldron.fody.csx", SearchOption.TopDirectoryOnly)
                    .Select(x => evaluator.LoadFile(x).AlignToInterface<IInterceptionScript>())
                    .Concat(
                        Directory.GetFiles(this.ProjectDirectoryPath, "*cauldron.fody.csxgz", SearchOption.TopDirectoryOnly)
                            .Select(x => Unzip(x))
                            .Select(x => evaluator.LoadFile(x).AlignToInterface<IInterceptionScript>())
                    )
                    .OrderBy(x => x.Priority)
                    .ToArray())
                {
                    this.Log(LogTypes.Info, "Executing script: " + script.Name);
                    script.Implement(builder);
                }
            }
        }
    }
}