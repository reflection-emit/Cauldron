using Mono.Cecil;
using System.IO;

namespace Cauldron.Interception.Cecilator
{
    public sealed class AssemblyDefinitionEx
    {
        internal AssemblyDefinitionEx(AssemblyDefinition assemblyDefinition, string filename)
        {
            this.AssemblyDefinition = assemblyDefinition;
            this.Filename = Path.GetFileName(filename);
        }

        public AssemblyDefinition AssemblyDefinition { get; private set; }

        public string Filename { get; private set; }
    }
}