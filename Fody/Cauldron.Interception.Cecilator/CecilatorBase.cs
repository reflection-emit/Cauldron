using Cauldron.Interception.Cecilator.Coders;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Cauldron.Interception.Cecilator
{
    public abstract class CecilatorBase : CecilatorObject
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly List<AssemblyDefinition> allAssemblies;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly List<TypeDefinition> allTypes;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly ModuleDefinition moduleDefinition;

        internal CecilatorBase(WeaverBase weaver)
        {
            this.Initialize(weaver.LogInfo, weaver.LogWarning, weaver.LogWarningPoint, weaver.LogError, weaver.LogErrorPoint);

            this.moduleDefinition = weaver.ModuleDefinition;

            var assemblies = this.GetAllAssemblyDefinitions(this.moduleDefinition.AssemblyReferences)
                  .Concat(weaver.References.Split(';').Select(x => LoadAssembly(x)))
                  .Concat(new AssemblyDefinition[] { this.moduleDefinition.Assembly });

            this.ReferenceCopyLocal = weaver.ReferenceCopyLocalPaths
                .Where(x => x.EndsWith(".dll"))
                .Select(x => new AssemblyDefinitionEx(LoadAssembly(x), x))
                .Where(x => x != null && !assemblies.Any(y => y.FullName.GetHashCode() == x.AssemblyDefinition.FullName.GetHashCode() && y.FullName == x.AssemblyDefinition.FullName))
                .ToArray();

            this.allAssemblies = assemblies
                .Concat(this.ReferenceCopyLocal.Select(x => x.AssemblyDefinition))
                .ToList();

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
            this.allTypes = this.allAssemblies
                .SelectMany(x => x.Modules)
                .Where(x => x != null)
                .SelectMany(x => x.Types)
                .Where(x => x != null)
                .Concat(this.moduleDefinition.Types)
                .Distinct(new TypeDefinitionEqualityComparer())
                .ToList();
            this.Log("-----------------------------------------------------------------------------");
            WeaverBase.AllTypes = this.allTypes;

            this.Identification = CodeBlocks.GenerateName();
        }

        internal CecilatorBase(CecilatorBase builderBase)
        {
            this.Initialize(builderBase);

            this.moduleDefinition = builderBase.moduleDefinition;
            this.allAssemblies = builderBase.allAssemblies;
            this.allTypes = builderBase.allTypes;
            this.ResourceNames = builderBase.ResourceNames;

            this.Identification = CodeBlocks.GenerateName();
        }

        public virtual string Identification { get; private set; }

        public bool IsUWP => this.IsReferenced("Windows.Foundation.UniversalApiContract");

        public AssemblyDefinitionEx[] ReferenceCopyLocal { get; private set; }

        public AssemblyDefinition[] ReferencedAssemblies =>
                            this.moduleDefinition.AssemblyReferences
                .Select(x => this.moduleDefinition.AssemblyResolver.Resolve(x)).ToArray();

        public List<string> ResourceNames { get; private set; } = new List<string>();

        public bool IsReferenced(string assemblyName) => this.allAssemblies.Any(x => x.Name.Name == assemblyName);

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

        private AssemblyDefinition LoadAssembly(string path)
        {
            try
            {
                return AssemblyDefinition.ReadAssembly(path);
            }
            catch (BadImageFormatException)
            {
                this.Log(LogTypes.Info, $"Info: a BadImageFormatException has occured while trying to retrieve information from '{path}'");
                return null;
            }
            catch (Exception e)
            {
                this.Log(e);
                return null;
            }
        }
    }
}