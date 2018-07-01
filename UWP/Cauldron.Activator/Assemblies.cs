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

            AddAssembly(EntryAssembly, false);

            if (AssembliesCore._referencedAssemblies == null)
                return;

            for (int i = 0; i < AssembliesCore._referencedAssemblies.Length; i++)
                AddAssembly(AssembliesCore._referencedAssemblies[i], false);
        }
    }
}