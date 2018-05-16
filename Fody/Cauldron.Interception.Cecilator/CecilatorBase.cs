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
    /// <exclude/>
    public abstract class CecilatorBase : CecilatorObject
    {
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly List<TypeDefinition> allTypes;

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly ModuleDefinition moduleDefinition;

        internal CecilatorBase(WeaverBase weaver)
        {
            this.Initialize(weaver.LogInfo, weaver.LogWarning, weaver.LogWarningPoint, weaver.LogError, weaver.LogErrorPoint);

            this.moduleDefinition = weaver.ModuleDefinition;

            this.ReferenceCopyLocal = weaver.ReferenceCopyLocalPaths
                .Where(x => x.EndsWith(".dll"))
                .Select(x => LoadAssembly(x))
                .Where(x => x != null)
                .ToArray();

            var referencedAssemblies = weaver
                .GetAllReferencedAssemblies(weaver.Resolve(this.moduleDefinition.AssemblyReferences))
                .Concat(weaver.References.Split(';').Select(x => LoadAssembly(x)))
                .Where(x => x != null).ToArray() as IEnumerable<AssemblyDefinition>;

            if (weaver.Config.Attribute("ReferenceCopyLocal").With(x => x == null ? true : bool.Parse(x.Value)))
                referencedAssemblies = referencedAssemblies.Concat(this.ReferenceCopyLocal);

            this.ReferencedAssemblies = referencedAssemblies.Distinct(new AssemblyDefinitionEqualityComparer()).ToArray();

            this.Log("-----------------------------------------------------------------------------");

            foreach (var item in this.ReferencedAssemblies)
                this.Log(LogTypes.Info, "<<Assembly>> " + item.Name);

            var resourceNames = new List<string>();
            foreach (var item in this.moduleDefinition.Resources)
            {
                this.Log(LogTypes.Info, "<<Resource>> " + item.Name + " " + item.ResourceType);
                if (item.ResourceType == ResourceType.Embedded)
                {
                    var embeddedResource = item as EmbeddedResource;
                    using (var stream = embeddedResource.GetResourceStream())
                    {
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
                    }
                }
            }

            this.ResourceNames = resourceNames.ToArray();

            this.allTypes = this.ReferencedAssemblies
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
            this.ReferencedAssemblies = builderBase.ReferencedAssemblies;
            this.allTypes = builderBase.allTypes;
            this.ResourceNames = builderBase.ResourceNames;

            this.Identification = CodeBlocks.GenerateName();
        }

        /// <summary>
        /// Gets a unique identification string for this object.
        /// </summary>
        public virtual string Identification { get; private set; }

        /// <summary>
        /// Gets a value that indicates if the weaved assembly is an UWP assembly or not.
        /// </summary>
        public bool IsUWP => this.IsReferenced("Windows.Foundation.UniversalApiContract");

        /// <summary>
        /// Gets an array of all the references marked as copy-local.
        /// </summary>
        public AssemblyDefinition[] ReferenceCopyLocal { get; private set; }

        /// <summary>
        /// Gets an array of referenced assemblies including the assemblies referenced by it's references.
        /// The array will include <see cref="CecilatorBase.ReferenceCopyLocal"/> assemblies as default.
        /// To deactivate this the attribute "ReferenceCopyLocal" in FodyWeaver.xml can be set to false.
        /// </summary>
        public AssemblyDefinition[] ReferencedAssemblies { get; private set; }

        /// <summary>
        /// Gets a list of names of all resources embedded in the assembly.
        /// </summary>
        public string[] ResourceNames { get; private set; }

        /// <summary>
        /// Checks if the assembly described by <paramref name="assemblyName"/> is referenced or not.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to check.</param>
        /// <returns>Return true if the assembly is referenced; otherwise false.</returns>
        public bool IsReferenced(string assemblyName) => this.ReferencedAssemblies.Any(x => x.Name.Name == assemblyName);

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