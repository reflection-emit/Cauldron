using System.Linq;
using System;
using System.IO;
using Cauldron.Core.Collections;
using Cauldron.Core.Extensions;
using Cauldron.Core;

#if NETFX_CORE

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Windows.UI.Xaml;

#else

using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

#endif

namespace Cauldron.Core
{
    /// <summary>
    /// Contains methods and properties that helps to manage and gather <see cref="Assembly"/> information
    /// </summary>
    public static class Assemblies
    {
        private static ConcurrentList<Assembly> _assemblies;
        private static ConcurrentList<TypesWithImplementedInterfaces> typesWithImplementedInterfaces;

        static Assemblies()
        {
            GetAllAssemblies();
            GetAllDefinedTypes();
            GetAllAssemblyAndResourceNameInfo();
        }

        /// <summary>
        /// Occures if the <see cref="_assemblies"/> has changed
        /// </summary>
        public static event EventHandler LoadedAssemblyChanged;

        /// <summary>
        /// Gets a collection of <see cref="AssemblyAndResourceNameInfo"/> that contains all fully qualified filename of embedded resources and thier corresponding <see cref="Assembly"/>
        /// </summary>
        public static ConcurrentList<AssemblyAndResourceNameInfo> AssemblyAndResourceNamesInfo { get; private set; }

        /// <summary>
        /// Gets a collection of classes loaded to the <see cref="Core.Assemblies"/>
        /// </summary>
        public static IEnumerable<TypeInfo> Classes { get { return ExportedTypes.Where(x => x.IsClass); } }

        /// <summary>
        /// Gets a collection of exported types found in the loaded <see cref="Assembly"/>
        /// </summary>
        public static IEnumerable<TypeInfo> ExportedTypes { get { return typesWithImplementedInterfaces.Select(x => x.typeInfo); } }

        /// <summary>
        /// Gets a colleciton of Interfaces found in the loaded <see cref="Assembly"/>
        /// </summary>
        public static IEnumerable<TypeInfo> Interfaces { get { return ExportedTypes.Where(x => x.IsInterface); } }

        /// <summary>
        /// Gets a value that determines if the <see cref="Assembly.GetEntryAssembly"/> or <see cref="Assembly.GetCallingAssembly"/> is in debug mode
        /// </summary>
        public static bool IsDebugging
        {
            get
            {
#if WINDOWS_UWP
                return Debugger.IsAttached;
#else

                var assembly = Assembly.GetEntryAssembly();

                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                var attrib = assembly.GetCustomAttribute<DebuggableAttribute>();
                return attrib == null ? false : attrib.IsJITTrackingEnabled;
#endif
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="Assembly"/> that is loaded to the <see cref="Core.Assemblies"/>
        /// </summary>
        public static ConcurrentList<Assembly> Known { get { return _assemblies; } }

#if WINDOWS_UWP

        /// <summary>
        /// Adds a new Assembly to the assembly collection
        /// </summary>
        /// <param name="assembly">The assembly to be added</param>
        /// <exception cref="NotSupportedException"><paramref name="assembly"/> is a dynamic assembly</exception>
        public static void AddAssembly(Assembly assembly)
        {
            if (assembly.IsDynamic)
                throw new NotSupportedException("Dynamic assemblies are not supported");

            if (_assemblies.Any(x => x.ManifestModule.Name == assembly.ManifestModule.Name))
                return;

            _assemblies.Add(assembly);

            var definedTypes = FilterTypes(assembly.DefinedTypes).Select(x => new TypesWithImplementedInterfaces
            {
                interfaces = x.ImplementedInterfaces.ToArray(),
                typeInfo = x
            });
            typesWithImplementedInterfaces.AddRange(definedTypes);

            AssemblyAndResourceNamesInfo.AddRange(assembly.GetManifestResourceNames().Select(x => new AssemblyAndResourceNameInfo(assembly, x)));

            LoadedAssemblyChanged?.Invoke(null, EventArgs.Empty);
        }

#endif

        /// <summary>
        /// Returns the first found <see cref="Assembly"/> that contains an embedded resource with the given resource name
        /// </summary>
        /// <param name="resourceInfoName">The end of the fully qualified name of the embedded resource</param>
        /// <returns>The first found <see cref="Assembly"/> otherwise returns null</returns>
        /// <exception cref="ArgumentNullException"><paramref name="resourceInfoName"/> is null</exception>
        /// <exception cref="ArgumentException">The <paramref name="resourceInfoName"/> parameter is an empty string ("").</exception>
        public static Assembly GetFirstAssemblyWithResourceName(string resourceInfoName)
        {
            if (resourceInfoName == null)
                throw new ArgumentNullException(nameof(resourceInfoName));

            if (resourceInfoName.Length == 0)
                throw new ArgumentException("The parameter is an empty string", nameof(resourceInfoName));

            foreach (var assembly in AssemblyAndResourceNamesInfo)
                if (assembly.Filename.EndsWith(resourceInfoName, StringComparison.OrdinalIgnoreCase))
                    return assembly.Assembly;

            return null;
        }

        /// <summary>
        /// Loads the specified manifest resource from this assembly.
        /// </summary>
        /// <param name="resourceInfoName">The end of the fully qualified name of the embedded resource</param>
        /// <returns>The manifest resource; or null if no resources were specified during compilation or if the resource is not visible to the caller.</returns>
        /// <exception cref="ArgumentNullException">The resourceInfoName parameter is null</exception>
        /// <exception cref="ArgumentException">The resourceInfoName parameter is an empty string</exception>
        /// <exception cref="FileLoadException">A file that was found could not be loaded.</exception>
        /// <exception cref="FileNotFoundException"><paramref name="resourceInfoName"/> was not found.</exception>
        /// <exception cref="BadImageFormatException"><paramref name="resourceInfoName"/> is not a valid assembly.</exception>
        /// <exception cref="NotImplementedException">Resource length is greater than <see cref="Int64.MaxValue"/></exception>
        public static byte[] GetManifestResource(string resourceInfoName)
        {
            if (resourceInfoName == null)
                throw new ArgumentNullException(nameof(resourceInfoName));

            if (resourceInfoName.Length == 0)
                throw new ArgumentException("The parameter is an empty string", nameof(resourceInfoName));

            var result = AssemblyAndResourceNamesInfo.FirstOrDefault(x => x.Filename.EndsWith(resourceInfoName, StringComparison.OrdinalIgnoreCase));

            if (result == null)
                throw new FileNotFoundException("resourceInfoName was not found.");

            using (var stream = result.Assembly.GetManifestResourceStream(result.Filename))
            {
                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.SetLength(stream.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Returns information about how the given resource has been persisted.
        /// </summary>
        /// <param name="resourceInfoName">The end of the fully qualified name of the embedded resource</param>
        /// <returns>An object that is populated with information about the resource's topology, or null if the resource is not found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="resourceInfoName"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="resourceInfoName"/> parameter is an empty string ("").</exception>
        public static ManifestResourceInfo GetManifestResourceInfo(string resourceInfoName)
        {
            if (resourceInfoName == null)
                throw new ArgumentNullException(nameof(resourceInfoName));

            if (resourceInfoName.Length == 0)
                throw new ArgumentException("The parameter is an empty string", nameof(resourceInfoName));

            var result = AssemblyAndResourceNamesInfo.FirstOrDefault(x => x.Filename.EndsWith(resourceInfoName, StringComparison.OrdinalIgnoreCase));

            if (result == null)
                return null;

            return result.Assembly.GetManifestResourceInfo(result.Filename);
        }

        /// <summary>
        /// Returns all information about how the given resource has been persisted.
        /// </summary>
        /// <param name="resourceInfoName">The end of the fully qualified name of the embedded resource</param>
        /// <returns>An <see cref="List{T}"/> object that is populated with information about the resource's topology</returns>
        /// <exception cref="ArgumentNullException"><paramref name="resourceInfoName"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="resourceInfoName"/> parameter is an empty string ("").</exception>
        public static List<AssemblyAndResourceNameInfo> GetManifestResources(string resourceInfoName)
        {
            if (resourceInfoName == null)
                throw new ArgumentNullException(nameof(resourceInfoName));

            if (resourceInfoName.Length == 0)
                throw new ArgumentException("The parameter is an empty string", nameof(resourceInfoName));

            return AssemblyAndResourceNamesInfo.Where(x => x.Filename.EndsWith(resourceInfoName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Tries to find/identify a <see cref="Type"/> by its name
        /// </summary>
        /// <param name="typeName">The name of the <see cref="Type"/></param>
        /// <returns>The <see cref="Type"/> that is defined by the parameter <paramref name="typeName"/></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeName"/> parameter is null</exception>
        /// <exception cref="ArgumentException">The <paramref name="typeName"/> parameter is an empty string</exception>
        public static Type GetTypeFromName(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));

            if (typeName.Length == 0)
                throw new ArgumentException("The parameter is an empty string", nameof(typeName));

            var result = ExportedTypes.FirstOrDefault(x => x.Name.EndsWith(typeName));

            if (result != null)
                return result.AsType();

            // Else let us loop through all known assemblies and look for the type
            foreach (var assembly in Core.Assemblies._assemblies)
            {
                var type = assembly.GetType(typeName);

                if (type != null)
                    return type;
            }

            Output.WriteLineError($"The type '{typeName}' defined by the parameter 'typeName' was not found");
            return null;
        }

        /// <summary>
        /// Returns all Types that implements the interface <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The interface <see cref="Type"/></typeparam>
        /// <returns>A colletion of <see cref="Type"/> that implements the interface <typeparamref name="T"/> otherwise null</returns>
        /// <exception cref="ArgumentException"><typeparamref name="T"/> is not an interface</exception>
        public static IEnumerable<TypeInfo> GetTypesImplementsInterface<T>()
        {
#if WINDOWS_UWP
            if (!typeof(T).GetTypeInfo().IsInterface)
#else
            if (!typeof(T).IsInterface)
#endif
                throw new ArgumentException(string.Format("The Type '{0}' is not an interface.", typeof(T).FullName));

            var interfaceType = typeof(T);
            var result = typesWithImplementedInterfaces.Where(x => x.interfaces.Length > 0 && x.interfaces.Contains(interfaceType));

            if (result == null)
                return null;

            return result.Select(x => x.typeInfo);
        }

#if WINDOWS_UWP
#else

        /// <summary>
        /// Loads the contents of all assemblies that matches the specified filter
        /// </summary>
        /// <param name="filter">
        /// The search string to match against the names of files in <see cref="ApplicationInfo.ApplicationPath"/>. This parameter can contain a combination of
        /// valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions.
        /// </param>
        /// <exception cref="FileLoadException">A file that was found could not be loaded</exception>
        public static void LoadAssembly(string filter = "*.dll")
        {
            foreach (var files in Directory.GetFiles(ApplicationInfo.ApplicationPath, filter, SearchOption.AllDirectories))
                LoadAssembly(files, false);
        }

        /// <summary>
        /// Loads the contents of an assembly file on the specified path.
        /// </summary>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <param name="relativPath">A value that indicates if the parameter <paramref name="path"/> is a relative path or not.</param>
        /// <exception cref="ArgumentException">The <paramref name="path"/> argument is not an absolute path.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="path"/> parameter is null.</exception>
        /// <exception cref="FileLoadException">A file that was found could not be loaded</exception>
        /// <exception cref="FileNotFoundException">The <paramref name="path"/> parameter is an empty string ("") or does not exist</exception>
        /// <exception cref="BadImageFormatException"><paramref name="path"/> is not a valid assembly.</exception>
        /// <returns>Returns true if the Assembly was loaded succesfully, otherwise false</returns>
        public static bool LoadAssembly(string path, bool relativPath = true)
        {
            try
            {
                var assemblyPath = string.Empty;

                if (relativPath)
                    assemblyPath = Path.Combine(ApplicationInfo.ApplicationPath, path);
                else
                    assemblyPath = path;

                var assembly = Assembly.LoadFile(assemblyPath);

                if (assembly.IsDynamic || Assemblies.Known.Any(x => x.ManifestModule.Name == assembly.ManifestModule.Name))
                    return true; // this is already loaded... No need to load again

                Assemblies.Known.Add(assembly);
                var definedTypes = FilterTypes(assembly.DefinedTypes).Select(x => new TypesWithImplementedInterfaces
                {
                    interfaces = x.ImplementedInterfaces.ToArray(),
                    typeInfo = x
                });
                typesWithImplementedInterfaces.AddRange(definedTypes);

                AssemblyAndResourceNamesInfo.AddRange(assembly.GetManifestResourceNames().Select(x => new AssemblyAndResourceNameInfo(assembly, x)));

                LoadedAssemblyChanged?.Invoke(null, EventArgs.Empty);

                return true;
            }
            catch (Exception e)
            {
                Output.WriteLineError("An error has occured while loading the assembly '{0}'\r\n{1}", path, e.Message);
                return false;
            }
        }

#endif

        private static void GetAllAssemblies()
        {
#if WINDOWS_UWP
            var assemblies = new List<Assembly>();
            assemblies.Add(Application.Current.GetType().GetTypeInfo().Assembly);
            assemblies.Add(typeof(Assemblies).GetTypeInfo().Assembly);

#else
            // Get all assemblies in AppDomain and add them to our list
            // TODO - This will not work in UWP and Core if compiled to native code
            var assemblies = new List<Assembly>();
            var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            assemblies.AddRange(domainAssemblies);
            assemblies.AddRange(domainAssemblies.SelectMany(x => x.GetReferencedAssemblies().Select(y => Assembly.Load(y))));
#endif
            _assemblies = new ConcurrentList<Assembly>(assemblies.Where(x => !x.IsDynamic));

            LoadedAssemblyChanged?.Invoke(null, EventArgs.Empty);
        }

        #region Private Methods

        private static IEnumerable<TypeInfo> FilterTypes(IEnumerable<TypeInfo> types) =>
            types.Where(x =>
                    !string.IsNullOrEmpty(x.Namespace) &&
                    !x.FullName.StartsWith("Microsoft.") &&
                    !x.FullName.StartsWith("System.") &&
                    !x.FullName.StartsWith("Windows.") &&
                    !x.FullName.StartsWith("MS.Internal.") &&
                    !x.FullName.StartsWith("<CrtImplementationDetails>") &&
                    !x.FullName.StartsWith("<CppImplementationDetails>") &&
                    !x.FullName.StartsWith("MS.") &&
                    !x.FullName.StartsWith("XamlGeneratedNamespace.") &&
                    !x.FullName.StartsWith("Castle.") &&
                    !x.FullName.StartsWith("Standard."));

        private static void GetAllAssemblyAndResourceNameInfo()
        {
            var list = new List<AssemblyAndResourceNameInfo>();

            foreach (var assembly in Core.Assemblies._assemblies)
            {
                var resources = assembly.GetManifestResourceNames().Select(x => new AssemblyAndResourceNameInfo(assembly, x));

                if (resources != null && resources.Count() > 0)
                    list.AddRange(resources);
            }

            AssemblyAndResourceNamesInfo = new ConcurrentList<AssemblyAndResourceNameInfo>(list.OrderBy(x => x.Filename));
        }

        private static void GetAllDefinedTypes()
        {
            List<TypeInfo> types = new List<TypeInfo>();

            foreach (var assembly in Core.Assemblies._assemblies)
            {
                try
                {
                    types.AddRange(assembly.ExportedTypes.Select(x => x.GetTypeInfo()));
                }
                catch (Exception e)
                {
                    // Just ignore this exception... Its actually not very interesting and cruicial
                    Output.WriteLineError("An error has occured while getting getting all types from assembly '{0}'\r\n{1}", assembly.FullName, e.Message);
                }
            }

            typesWithImplementedInterfaces = new ConcurrentList<TypesWithImplementedInterfaces>(
                FilterTypes(types) /* Exclude everything that is in the namespace microsoft and system. We dont need those for anything */
                    .Select(x => new TypesWithImplementedInterfaces
                    {
                        typeInfo = x,
                        interfaces = x.ImplementedInterfaces.ToArray()
                    }));
        }

        #endregion Private Methods

        private class TypesWithImplementedInterfaces
        {
            public Type[] interfaces;
            public TypeInfo typeInfo;

            public override string ToString()
            {
                return typeInfo.ToString() + " (" + interfaces.Count() + ")";
            }
        }
    }
}