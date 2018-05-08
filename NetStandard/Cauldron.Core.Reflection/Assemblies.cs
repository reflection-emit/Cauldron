using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Cauldron.Core.Reflection
{
    using Cauldron.Core.Diagnostics;

    /// <summary>
    /// Contains methods and properties that helps to manage and gather <see cref="Assembly"/> information
    /// </summary>
    public static partial class Assemblies
    {
        /// <summary>
        /// Gets a collection of classes loaded to the AppDomain
        /// </summary>
        public static IEnumerable<Type> Classes => _assemblies.SelectMany(x => x.ExportedTypes).Where(x => x.IsClass);

        /// <summary>
        /// Gets a colleciton of Interfaces found in the AppDomain
        /// </summary>
        public static IEnumerable<Type> Interfaces => _assemblies.SelectMany(x => x.ExportedTypes).Where(x => x.IsInterface);

        /// <summary>
        /// Gets a value that determines if the <see cref="Assembly.GetEntryAssembly"/> or <see
        /// cref="Assembly.GetCallingAssembly"/> is in debug mode
        /// </summary>
        public static bool IsDebugging
        {
            get
            {
                var attrib = EntryAssembly.GetCustomAttribute<System.Diagnostics.DebuggableAttribute>();
                return attrib == null ? false : attrib.IsJITTrackingEnabled;
            }
        }

        /// <summary>
        /// Loads the contents of all assemblies that matches the specified filter
        /// </summary>
        /// <param name="directory">The directory where the assemblies are located</param>
        /// <param name="filter">
        /// The search string to match against the names of files in <paramref name="directory"/>.
        /// This parameter can contain a combination of valid literal path and wildcard (* and ?)
        /// characters, but doesn't support regular expressions.
        /// </param>
        /// <exception cref="FileLoadException">A file that was found could not be loaded</exception>
        public static void LoadAssembly(DirectoryInfo directory, string filter = "*.dll")
        {
            if (!directory.Exists)
                throw new DirectoryNotFoundException("Unable to find directory: " + directory.FullName);

            var files = directory.GetFiles(filter);
            for (int i = 0; i < files.Length; i++)
                AddAssembly(Assembly.LoadFile(files[i].FullName), true);
        }

        private static void GetAllAssemblies()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ResolveAssembly;
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            AddAssembly(EntryAssembly, false);

            if (AssembliesCore._referencedAssemblies == null)
            {
                var assemblies = new List<Assembly>();
                var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

                Parallel.For(0, allAssemblies.Length, i =>
                {
                    var assembly = allAssemblies[i];
                    if (AddAssembly(assembly, false) != null)
                    {
                        var referencedAssemblies = assembly.GetReferencedAssemblies();
                        for (int c = 0; c < referencedAssemblies.Length; c++)
                            LoadAssembly(referencedAssemblies[c]);
                    }
                });

                return;
            }

            for (int i = 0; i < AssembliesCore._referencedAssemblies.Length; i++)
                AddAssembly(AssembliesCore._referencedAssemblies[i], false);
        }

        private static Assembly ResolveAssembly(object sender, ResolveEventArgs e)
        {
            if (e.RequestingAssembly == null)
                Debug.WriteLine($"Assembly requesting for '{e.Name}'");
            else
                Debug.WriteLine($"Assembly '{e.RequestingAssembly.FullName}' requesting for '{e.Name}'");

            var assembly = _assemblies?.FirstOrDefault(x => x.FullName.GetHashCode() == e.Name.GetHashCode() && x.FullName == e.Name);

            // The following resolve tries can only be successfull if the dll's name is the same as
            // the simple Assembly name

            try
            {
                // Try to load it from application directory
                if (assembly == null)
                {
                    var file = Path.Combine(ApplicationInfo.ApplicationPath.FullName, $"{new AssemblyName(e.Name).Name}.dll");
                    if (File.Exists(file))
                        return AddAssembly(Assembly.LoadFile(file), false);
                }

                // Try to load it from current domain's base directory
                var assemblyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{new AssemblyName(e.Name).Name}.dll");
                if (File.Exists(assemblyFile))
                    return AddAssembly(Assembly.LoadFile(assemblyFile), false);

                // As last resort try to load it from this Assembly's directory
                assemblyFile = Path.Combine(Path.GetDirectoryName(typeof(Assemblies).Assembly.Location), $"{new AssemblyName(e.Name).Name}.dll");
                if (File.Exists(assemblyFile))
                    return AddAssembly(Assembly.LoadFile(assemblyFile), false);

                return assembly;
            }
            catch
            {
                return null;
            }
        }
    }
}