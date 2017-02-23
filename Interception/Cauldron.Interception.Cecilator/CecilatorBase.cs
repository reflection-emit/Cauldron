using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public abstract class CecilatorBase
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly IEnumerable<AssemblyDefinition> allAssemblies;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly IEnumerable<TypeDefinition> allTypes;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ModuleDefinition moduleDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<string> logError;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<string> logInfo;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<string> logWarning;

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

            this.Identification = GenerateName();
        }

        internal CecilatorBase(CecilatorBase builderBase)
        {
            this.logError = builderBase.logError;
            this.logInfo = builderBase.logInfo;
            this.logWarning = builderBase.logWarning;
            this.moduleDefinition = builderBase.moduleDefinition;
            this.allAssemblies = builderBase.allAssemblies;
            this.allTypes = builderBase.allTypes;

            this.Identification = GenerateName();
        }

        public string Identification { get; private set; }

        public static string GenerateName() => Path.GetRandomFileName().Replace(".", DateTime.Now.Second.ToString());

        internal IEnumerable<TypeReference> GetBaseClasses(TypeReference type)
        {
            var typeDef = type.Resolve();

            if (typeDef != null && typeDef.BaseType != null)
            {
                yield return typeDef.BaseType;
                foreach (var item in GetBaseClasses(typeDef.BaseType))
                    yield return item;
            }
        }

        internal IEnumerable<TypeReference> GetInterfaces(TypeReference type)
        {
            var typeDef = type.Resolve();

            if (typeDef == null)
                return new TypeReference[0];

            if (typeDef.Interfaces != null && typeDef.Interfaces.Count > 0)
                return type.Recursive(x => x.Resolve().Interfaces).Select(x => x.ResolveType(type));

            if (typeDef.BaseType != null)
                return GetInterfaces(typeDef.BaseType);

            return new TypeReference[0];
        }

        internal IEnumerable<TypeReference> GetNestedTypes(TypeReference type)
        {
            var typeDef = type.Resolve();

            if (typeDef == null)
                return new TypeReference[0];

            if (typeDef.NestedTypes != null && typeDef.NestedTypes.Count > 0)
                return type.Recursive(x => x.Resolve().NestedTypes).Select(x => x.ResolveType(type));

            if (typeDef.BaseType != null)
                return GetNestedTypes(typeDef.BaseType);

            return new TypeReference[0];
        }

        internal TypeDefinition GetTypeDefinition(Type type)
        {
            var result = this.allTypes.FirstOrDefault(x => x.FullName == type.FullName);

            if (result == null)
                throw new Exception($"Unable to proceed. The type '{type.FullName}' was not found.");

            return result;
        }

        internal void LogError(object value)
        {
            if (value is string)
                this.logError(value as string);
            else
                this.logError(value.ToString());
        }

        internal void LogInfo(object value)
        {
            if (value is string)
                this.logInfo(value as string);
            else
                this.logInfo(value.ToString());
        }

        internal void LogWarning(object value)
        {
            if (value is string)
                this.logWarning(value as string);
            else
                this.logWarning(value.ToString());
        }

        protected IEnumerable<Instruction> TypeOf(ILProcessor processor, TypeReference type)
        {
            return new Instruction[] {
                processor.Create(OpCodes.Ldtoken, type),
                processor.Create(OpCodes.Call,
                    this.moduleDefinition.Import(
                        this.GetTypeDefinition(typeof(Type))
                            .Methods.FirstOrDefault(x=>x.Name == "GetTypeFromHandle" && x.Parameters.Count == 1)))
            };
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