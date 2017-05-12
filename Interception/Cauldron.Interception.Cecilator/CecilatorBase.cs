using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
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
        protected readonly ModuleDefinition moduleDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Action<string> logError;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Action<string> logInfo;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Action<string> logWarning;

        internal CecilatorBase(IWeaver weaver)
        {
            this.logError = weaver.LogError;
            this.logInfo = weaver.LogInfo;
            this.logWarning = weaver.LogWarning;
            this.moduleDefinition = weaver.ModuleDefinition;

            var assemblies = this.GetAllAssemblyDefinitions(this.moduleDefinition.AssemblyReferences)
                  .Concat(new AssemblyDefinition[] { this.moduleDefinition.Assembly });

            this.UnusedReference = weaver.ReferenceCopyLocalPaths
                .Where(x => x.EndsWith(".dll"))
                .Select(x =>
                {
                    try
                    {
                        return new AssemblyDefinitionEx(AssemblyDefinition.ReadAssembly(x), x);
                    }
                    catch (BadImageFormatException e)
                    {
                        this.LogInfo(e.Message + " " + x);
                        return null;
                    }
                    catch (Exception e)
                    {
                        this.LogWarning(e.Message);
                        return null;
                    }
                })
                .Where(x => x != null && !assemblies.Any(y => y.FullName.GetHashCode() == x.AssemblyDefinition.FullName.GetHashCode() && y.FullName == x.AssemblyDefinition.FullName))
                .ToArray();

            this.allAssemblies = assemblies.Concat(this.UnusedReference.Select(x => x.AssemblyDefinition)).ToArray();

            this.logInfo("-----------------------------------------------------------------------------");

            foreach (var item in allAssemblies)
                this.logInfo("<<Assembly>> " + item.Name);

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
        public AssemblyDefinitionEx[] UnusedReference { get; private set; }

        public static string GenerateName() => Path.GetRandomFileName().Replace(".", DateTime.Now.Second.ToString());

        internal bool AreReferenceAssignable(BuilderType type, BuilderType toBeAssigned)
        {
            if ((toBeAssigned == null && !type.IsValueType) || type == toBeAssigned || (!type.typeDefinition.IsValueType && !toBeAssigned.typeDefinition.IsValueType && type.IsAssignableFrom(toBeAssigned)) || (type.IsInterface && toBeAssigned.typeReference == this.moduleDefinition.TypeSystem.Object))
                return true;

            return false;
        }

        internal bool AreReferenceAssignable(TypeReference type, TypeReference toBeAssigned)
        {
            if (
                (toBeAssigned == null && !type.IsValueType) ||
                type == toBeAssigned ||
                (!type.IsValueType && !toBeAssigned.IsValueType && type.IsAssignableFrom(toBeAssigned)) ||
                (type.Resolve().IsInterface && toBeAssigned == this.moduleDefinition.TypeSystem.Object) ||
                type.FullName == toBeAssigned.FullName)
                return true;

            return false;
        }

        internal TypeDefinition GetTypeDefinition(Type type)
        {
            var result = this.allTypes.Get(type.FullName);

            if (result == null)
                throw new Exception($"Unable to proceed. The type '{type.FullName}' was not found.");

            return this.moduleDefinition.ImportReference(type).Resolve();
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
                    this.moduleDefinition.ImportReference(
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