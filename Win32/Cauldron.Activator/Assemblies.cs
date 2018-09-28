using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cauldron.Reflection
{
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
            void addAssemblies()
            {
                for (int i = 0; i < AssembliesCore._referencedAssemblies.Length; i++)
                    AddAssembly(AssembliesCore._referencedAssemblies[i], false);
            }

            AddAssembly(EntryAssembly, false);

            if (AssembliesCore._referencedAssemblies == null)
            {
                var cauldronHelper = EntryAssembly.GetType("CauldronInterceptionHelper")?.GetMethod("GetReferencedAssemblies", BindingFlags.Public | BindingFlags.Static);

                if (cauldronHelper != null)
                {
                    if (cauldronHelper.Invoke(null, null) is Assembly[] refAssemblies && refAssemblies.Length > 0)
                    {
                        AssembliesCore._referencedAssemblies = refAssemblies;
                        addAssemblies();
                        return;
                    }
                }

                var assemblies = new List<Assembly>();
                var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

                for (int i = 0; i < allAssemblies.Length; i++)
                {
                    var assembly = allAssemblies[i];
                    if (AddAssembly(assembly, false) != null)
                    {
                        var referencedAssemblies = assembly.GetReferencedAssemblies();
                        for (int c = 0; c < referencedAssemblies.Length; c++)
                            LoadAssembly(referencedAssemblies[c]);
                    }
                };

                return;
            }

            addAssemblies();
        }
    }
}