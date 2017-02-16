using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class BuilderBase
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected IEnumerable<AssemblyDefinition> allAssemblies;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected IEnumerable<TypeDefinition> allTypes;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected Action<string> logError;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected Action<string> logInfo;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected Action<string> logWarning;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ModuleDefinition moduleDefinition;

        internal BuilderBase(IWeaver weaver)
        {
            this.logError = weaver.LogError;
            this.logInfo = weaver.LogInfo;
            this.logWarning = weaver.LogWarning;
            this.moduleDefinition = weaver.ModuleDefinition;

            this.allAssemblies = this.GetAllAssemblyDefinitions(this.moduleDefinition.AssemblyReferences)
                  .Concat(new AssemblyDefinition[] { this.moduleDefinition.Assembly }).ToArray();

            this.logInfo("-----------------------------------------------------------------------------");

            foreach (var item in allAssemblies)
                this.logInfo("<<Assembly>> " + item.FullName);

            this.allTypes = this.allAssemblies.SelectMany(x => x.Modules).Where(x => x != null).SelectMany(x => x.Types).Where(x => x != null).Concat(this.moduleDefinition.Types).ToArray();
            this.logInfo("-----------------------------------------------------------------------------");
        }

        internal BuilderBase(BuilderBase builderBase)
        {
            this.logError = builderBase.logError;
            this.logInfo = builderBase.logInfo;
            this.logWarning = builderBase.logWarning;
            this.moduleDefinition = builderBase.moduleDefinition;
            this.allAssemblies = builderBase.allAssemblies;
            this.allTypes = builderBase.allTypes;
        }

        internal IEnumerable<TypeReference> GetInterfaces(TypeDefinition type)
        {
            var result = new List<TypeReference>();

            if (type.Interfaces != null && type.Interfaces.Count > 0)
                result.AddRange(type.Interfaces.Select(x => x.Resolve()).Where(x => x != null));

            if (type.BaseType != null)
                result.AddRange(GetInterfaces(type.BaseType.Resolve()).Where(x => x != null));

            return result;
        }

        internal bool ImplementsInterface(TypeDefinition type, string interfaceName) => GetInterfaces(type).Any(x => x.FullName == interfaceName);

        private void GetAllAssemblyDefinitions(IEnumerable<AssemblyNameReference> target, List<AssemblyDefinition> result)
        {
            result.AddRange(target.Select(x => this.moduleDefinition.AssemblyResolver.Resolve(x)).Where(x => x != null));

            foreach (var item in target)
            {
                var assembly = this.moduleDefinition.AssemblyResolver.Resolve(item);

                if (assembly == null)
                    continue;

                if (result.Contains(assembly))
                    continue;

                result.Add(assembly);

                if (assembly.MainModule.HasAssemblyReferences)
                {
                    foreach (var a in assembly.Modules)
                        GetAllAssemblyDefinitions(a.AssemblyReferences, result);
                }
            }
        }

        private IEnumerable<AssemblyDefinition> GetAllAssemblyDefinitions(IEnumerable<AssemblyNameReference> target)
        {
            var result = new List<AssemblyDefinition>();
            result.AddRange(target.Select(x => this.moduleDefinition.AssemblyResolver.Resolve(x)).Where(x => x != null));

            foreach (var item in target)
            {
                var assembly = this.moduleDefinition.AssemblyResolver.Resolve(item);

                if (assembly == null)
                    continue;

                result.Add(assembly);

                if (assembly.MainModule.HasAssemblyReferences)
                {
                    foreach (var a in assembly.Modules)
                        this.GetAllAssemblyDefinitions(a.AssemblyReferences, result);
                }
            }

            return result.Distinct(new AssemblyDefinitionEqualityComparer());
        }
    }
}