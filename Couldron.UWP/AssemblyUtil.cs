using Couldron.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Couldron
{
    /// <summary>
    /// Contains utilities that helps to manage and gather <see cref="Assembly"/> information
    /// </summary>
    public static partial class AssemblyUtil
    {
        /// <summary>
        /// Returns all Types that implements the interface <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The interface <see cref="Type"/></typeparam>
        /// <returns>A colletion of <see cref="Type"/> that implements the interface <typeparamref name="T"/> otherwise null</returns>
        /// <exception cref="ArgumentException"><typeparamref name="T"/> is not an interface</exception>
        public static IEnumerable<TypeInfo> GetTypesImplementsInterface<T>()
        {
            if (!typeof(T).GetTypeInfo().IsInterface)
                throw new ArgumentException(string.Format("The Type '{0}' is not an interface.", typeof(T).FullName));

            var interfaceType = typeof(T);
            var result = typesWithImplementedInterfaces.Where(x => x.interfaces.Length > 0 && x.interfaces.Contains(interfaceType));

            if (result == null)
                return null;

            return result.Select(x => x.typeInfo);
        }

        /// <summary>
        /// Loads an assembly given its <see cref="AssemblyName"/>
        /// </summary>
        /// <param name="assemblyName">The object that describes the assembly to be loaded.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assemblyName"/> is null</exception>
        /// <exception cref="FileLoadException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch the base
        /// class exception, System.IO.IOException, instead.A file that was found could not
        /// be loaded.
        /// </exception>
        /// <exception cref="FileNotFoundException"><paramref name="assemblyName"/> is not found</exception>
        /// <exception cref="BadImageFormatException">
        /// <paramref name="assemblyName"/> is not a valid assembly. -or-Version 2.0 or later of the common language
        /// runtime is currently loaded and assemblyRef was compiled with a later version.
        /// </exception>
        public static void LoadAssembly(AssemblyName assemblyName)
        {
            var assembly = Assembly.Load(assemblyName);

            typesWithImplementedInterfaces.AddRange(
                assembly.DefinedTypes.Select(x => new TypesWithImplementedInterfaces
                {
                    interfaces = x.ImplementedInterfaces.ToArray(),
                    typeInfo = x
                }));

            AssemblyAndResourceNamesInfo.AddRange(assembly.GetManifestResourceNames().Select(x => new AssemblyAndResourceNameInfo(assembly, x)));
        }

        private static void GetAllAssemblies()
        {
            var assemblies = new List<Assembly>();

            foreach (var file in Windows.ApplicationModel.Package.Current.InstalledLocation.GetFilesAsync().GetResults())
            {
                if (file.FileType == ".exe")
                {
                    try
                    {
                        AssemblyName name = new AssemblyName() { Name = Path.GetFileNameWithoutExtension(file.Name) };
                        Assembly asm = Assembly.Load(name);
                        assemblies.Add(asm);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Error: " + e.Message);
                    }
                }
            }

            // add the couldron assembly also
            assemblies.Add(Assembly.Load(new AssemblyName("Couldron")));
            Assemblies = new ConcurrentList<Assembly>(assemblies.Distinct());
        }
    }
}