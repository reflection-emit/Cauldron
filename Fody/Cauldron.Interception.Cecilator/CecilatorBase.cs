using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Cauldron.Interception.Cecilator
{
    public abstract class CecilatorBase : CecilatorObject
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly IEnumerable<AssemblyDefinition> allAssemblies;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly IEnumerable<TypeDefinition> allTypes;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly ModuleDefinition moduleDefinition;

        internal CecilatorBase(WeaverBase weaver)
        {
            this.Initialize(weaver.LogInfo, weaver.LogWarning, weaver.LogWarningPoint, weaver.LogError, weaver.LogErrorPoint);

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
                        this.Log(e, x);
                        return null;
                    }
                    catch (Exception e)
                    {
                        this.Log(e);
                        return null;
                    }
                })
                .Where(x => x != null && !assemblies.Any(y => y.FullName.GetHashCode() == x.AssemblyDefinition.FullName.GetHashCode() && y.FullName == x.AssemblyDefinition.FullName))
                .ToArray();

            this.allAssemblies = assemblies.Concat(this.UnusedReference.Select(x => x.AssemblyDefinition)).ToArray();

            this.Log("-----------------------------------------------------------------------------");

            foreach (var item in allAssemblies)
                this.Log("<<Assembly>> " + item.Name);

            foreach (var item in this.moduleDefinition.Resources)
            {
                this.Log("<<Resource>> " + item.Name + " " + item.ResourceType);
                if (item.ResourceType == ResourceType.Embedded)
                {
                    var embeddedResource = item as EmbeddedResource;
                    using (var stream = embeddedResource.GetResourceStream())
                    {
                        var resourceNames = new List<string>();
                        var bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                        if (bytes[0] == 0xce && bytes[1] == 0xca && bytes[2] == 0xef && bytes[3] == 0xbe)
                        {
                            var resourceCount = BitConverter.ToInt16(bytes.GetBytes(160, 2).Reverse().ToArray(), 0);

                            if (resourceCount > 0)
                            {
                                var startPoint = resourceCount * 8 + 180;

                                for (int i = 0; i < resourceCount; i++)
                                {
                                    var length = (int)bytes[startPoint];
                                    var data = Encoding.Unicode.GetString(bytes, startPoint + 1, length).Trim();
                                    startPoint += length + 5;
                                    resourceNames.Add(data);
                                    this.Log("             " + data);
                                }
                            }
                        }
                        this.ResourceNames.AddRange(resourceNames);
                    }
                }
            }
            this.allTypes = this.allAssemblies.SelectMany(x => x.Modules).Where(x => x != null).SelectMany(x => x.Types).Where(x => x != null).Concat(this.moduleDefinition.Types).ToArray();
            this.Log("-----------------------------------------------------------------------------");
            WeaverBase.AllTypes = this.allTypes;

            this.Identification = GenerateName();
        }

        internal CecilatorBase(CecilatorBase builderBase)
        {
            this.Initialize(builderBase);

            this.moduleDefinition = builderBase.moduleDefinition;
            this.allAssemblies = builderBase.allAssemblies;
            this.allTypes = builderBase.allTypes;
            this.ResourceNames = builderBase.ResourceNames;

            this.Identification = GenerateName();
        }

        public string Identification { get; private set; }

        public AssemblyDefinition[] ReferencedAssemblies =>
            this.moduleDefinition.AssemblyReferences
                .Select(x => this.moduleDefinition.AssemblyResolver.Resolve(x)).ToArray();

        public List<string> ResourceNames { get; private set; } = new List<string>();
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

            return this.moduleDefinition.ImportReference(type).Resolve() ?? result;
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
                try
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
                catch (OutOfMemoryException)
                {
                    this.Log(LogTypes.Warning, $"Unable to load '{item.FullName}'. This may cause on the resulting assembly. Please make sure Fody's 'VerifyAssembly' switch is set to 'True'.");
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