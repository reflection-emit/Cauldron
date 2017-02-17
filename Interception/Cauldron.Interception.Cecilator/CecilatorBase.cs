using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public abstract class CecilatorBase
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

        internal CecilatorBase(IWeaver weaver)
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

        internal CecilatorBase(CecilatorBase builderBase)
        {
            this.logError = builderBase.logError;
            this.logInfo = builderBase.logInfo;
            this.logWarning = builderBase.logWarning;
            this.moduleDefinition = builderBase.moduleDefinition;
            this.allAssemblies = builderBase.allAssemblies;
            this.allTypes = builderBase.allTypes;
        }

        internal IEnumerable<TypeReference> GetBaseClasses(TypeReference type)
        {
            var typeDef = type.Resolve();

            if (typeDef.BaseType != null)
            {
                yield return typeDef.BaseType;
                foreach (var item in GetBaseClasses(typeDef.BaseType))
                    yield return item;
            }
        }

        internal IEnumerable<TypeReference> GetInterfaces(TypeReference type)
        {
            var typeDef = type.Resolve();

            if (typeDef.Interfaces != null && typeDef.Interfaces.Count > 0)
            {
                for (int i = 0; i < typeDef.Interfaces.Count; i++)
                {
                    yield return typeDef.Interfaces[i].ResolveType(type);
                    foreach (var item in GetInterfaces(typeDef.Interfaces[i]))
                        yield return item.ResolveType(type);
                }
            }

            if (typeDef.BaseType != null)
            {
                foreach (var item in GetInterfaces(typeDef.BaseType))
                    yield return item;
            }
        }

        internal IEnumerable<TypeReference> GetNestedTypes(TypeReference type)
        {
            var typeDef = type.Resolve();

            if (typeDef.NestedTypes != null && typeDef.NestedTypes.Count > 0)
            {
                for (int i = 0; i < typeDef.NestedTypes.Count; i++)
                {
                    yield return typeDef.NestedTypes[i].ResolveType(type);
                    foreach (var item in GetNestedTypes(typeDef.NestedTypes[i]))
                        yield return item.ResolveType(type);
                }
            }

            if (typeDef.BaseType != null)
            {
                foreach (var item in GetNestedTypes(typeDef.BaseType))
                    yield return item;
            }
        }

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