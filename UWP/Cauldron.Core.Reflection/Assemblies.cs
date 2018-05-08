using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Cauldron.Core.Reflection
{
    /// <summary>
    /// Contains methods and properties that helps to manage and gather <see cref="Assembly"/> information
    /// </summary>
    public static partial class Assemblies
    {
        /// <summary>
        /// Gets a collection of classes loaded to the AppDomain
        /// </summary>
        public static IEnumerable<Type> Classes => _assemblies.SelectMany(x => x.ExportedTypes).Where(x => x.GetTypeInfo().IsClass);

        /// <summary>
        /// Gets a colleciton of Interfaces found in the AppDomain
        /// </summary>
        public static IEnumerable<Type> Interfaces => _assemblies.SelectMany(x => x.ExportedTypes).Where(x => x.GetTypeInfo().IsInterface);

        /// <summary>
        /// Gets a value that determines if the Debugger is attached to the process
        /// </summary>
        public static bool IsDebugging => Debugger.IsAttached;

        private static void GetAllAssemblies()
        {
            if (EntryAssembly == null)
                return;

            var cauldron = EntryAssembly
                .GetType("<Cauldron>", false, false)?
                .GetMethod("GetReferencedAssemblies", BindingFlags.Public | BindingFlags.Static);

            _assemblies.Add(EntryAssembly);

            if (cauldron != null)
            {
                var assemblies = cauldron.Invoke(null, null) as Assembly[];
                if (assemblies == null)
                    return;

                for (int i = 0; i < assemblies.Length; i++)
                    AddAssembly(assemblies[i]);
            }
        }
    }
}