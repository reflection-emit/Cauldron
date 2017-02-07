using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed class ModuleWeaver
    {
        private List<Type> weavers = new List<Type>
        {
            typeof(MethodInterceptorWeaver),
            typeof(PropertyInterceptorWeaver)
        };

        public IAssemblyResolver AssemblyResolver { get; set; }

        public Action<string> LogError { get; set; }

        public Action<string> LogInfo { get; set; }

        public Action<string> LogWarning { get; set; }

        public ModuleDefinition ModuleDefinition { get; set; }

        public void Execute()
        {
            try
            {
                Extensions.Asemblies = this.GetAssemblies();
                Extensions.Types = Extensions.Asemblies.SelectMany(x => x.Modules).SelectMany(x => x.Types).ToArray();
                Extensions.ModuleDefinition = this.ModuleDefinition;

                foreach (var weaverType in this.weavers)
                {
                    var weaver = Activator.CreateInstance(weaverType, this) as ModuleWeaverBase;
                    weaver.Implement();
                }
            }
            catch (Exception e)
            {
                this.LogError(e.GetStackTrace());
                this.LogError(e.StackTrace);
            }
        }

        internal TypeDefinition GetType(string typeName) => this.GetCauldronCore().Modules.SelectMany(x => x.Types).FirstOrDefault(x => x.FullName == typeName);

        private IEnumerable<AssemblyDefinition> GetAssemblies() => this.ModuleDefinition.AssemblyReferences.Select(x => this.AssemblyResolver.Resolve(x)).Concat(new AssemblyDefinition[] { this.ModuleDefinition.Assembly }).ToArray();

        private AssemblyDefinition GetCauldronCore()
        {
            var assemblyNameReference = this.ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Cauldron.Core");
            if (assemblyNameReference == null)
                throw new Exception($"The project {this.ModuleDefinition.Name} does not reference to 'Cauldron.Core'. Please add Cauldron.Core to your project.");
            return this.AssemblyResolver.Resolve(assemblyNameReference);
        }
    }
}