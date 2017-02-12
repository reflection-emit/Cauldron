using System.Linq;
using System;
using System.IO;
using Cauldron.Core.Collections;
using Cauldron.Core.Extensions;

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#if NETFX_CORE

using Windows.UI.Xaml;

#elif NETCORE

using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

#endif

namespace Cauldron.Core
{
    /// <summary>
    /// Contains methods and properties that helps to manage and gather <see cref="Assembly"/> information
    /// </summary>
    public static partial class Assemblies
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
        /// Occures if the assembly dictionary has changed
        /// </summary>
        public static event EventHandler<AssemblyAddedEventArgs> LoadedAssemblyChanged;

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
        /// Gets a collection of <see cref="Assembly"/> that is loaded to the <see cref="Core.Assemblies"/>
        /// </summary>
        public static ConcurrentList<Assembly> Known { get { return _assemblies; } }

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

            var result = GetManifestResource(x => x.Filename.EndsWith(resourceInfoName, StringComparison.OrdinalIgnoreCase));

            if (result == null)
                throw new FileNotFoundException("resourceInfoName was not found.");

            return result;
        }

        /// <summary>
        /// Loads the specified manifest resource from this assembly.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns>The manifest resource; or null if no resources were specified during compilation or if the resource is not visible to the caller.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> parameter is null</exception>
        /// <exception cref="ArgumentException">The <paramref name="selector"/> parameter is an empty string</exception>
        /// <exception cref="FileLoadException">A file that was found could not be loaded.</exception>
        /// <exception cref="NotImplementedException">Resource length is greater than <see cref="Int64.MaxValue"/></exception>
        public static byte[] GetManifestResource(Func<AssemblyAndResourceNameInfo, bool> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var result = AssemblyAndResourceNamesInfo.FirstOrDefault(x => selector(x));

            if (result == null)
                return null;

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
#if WINDOWS_UWP || NETCORE
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

        private static void GetAllAssemblies()
        {
#if WINDOWS_UWP
            var assemblies = new List<Assembly>();
            assemblies.Add(Application.Current.GetType().GetTypeInfo().Assembly);
            assemblies.Add(typeof(Assemblies).GetTypeInfo().Assembly);
#elif NETCORE

            var assemblies = new List<Assembly>();
            assemblies.Add(Assembly.GetEntryAssembly());
            assemblies.Add(typeof(Assemblies).GetTypeInfo().Assembly);
#else

            // Get all assemblies in AppDomain and add them to our list
            // TODO - This will not work in UWP and Core if compiled to native code
            var assemblies = new List<Assembly>();
#endif

#if NETCORE
            AssemblyLoadContext.Default.Resolving += ResolveAssembly;
#elif ANDROID
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
#elif DESKTOP
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ResolveAssembly;
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
#endif

#if DESKTOP || ANDROID || NETCORE
#if NETCORE
            foreach (var assembly in GetAssemblies())
#else
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
#endif
            {
                try
                {
                    assemblies.Add(assembly);

                    // Get all referenced Assembly
                    foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
                    {
                        // We have to do this one by one instead of linq in order to be able to react to loading errors
                        try
                        {
                            assemblies.Add(Assembly.Load(referencedAssembly));
                        }
                        catch (Exception e)
                        {
                            var message = "Error while loading an assembly" + e.Message;
                            Console.WriteLine(message);
                            Output.WriteLineError(message);
                        }
                    }
                }
                catch (Exception e)
                {
                    var message = "Error while loading an assembly" + e.Message;
                    Console.WriteLine(message);
                    Output.WriteLineError(message);
                }
            }

#endif
            _assemblies = new ConcurrentList<Assembly>(assemblies.Where(x => !x.IsDynamic).Distinct());
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
                    !x.FullName.StartsWith("PostSharp.") &&
                    !x.FullName.StartsWith("Newtonsoft.Json.") &&
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

        /// <summary>
        /// Contains data of the <see cref="LoadedAssemblyChanged"/> event.
        /// </summary>
        public sealed class AssemblyAddedEventArgs : EventArgs
        {
            internal AssemblyAddedEventArgs(Assembly assembly, Type[] types)
            {
                this.Types = types;
                this.Assembly = assembly;
            }

            /// <summary>
            /// Gets the assembly that has been added to the known assembly collection
            /// </summary>
            public Assembly Assembly { get; private set; }

            /// <summary>
            /// Gets a collection of types that is defined in the assembly
            /// </summary>
            public IEnumerable<Type> Types { get; private set; }
        }

        private class TypesWithImplementedInterfaces
        {
            public Type[] interfaces;
            public TypeInfo typeInfo;

            public override string ToString() => typeInfo.ToString() + " (" + interfaces.Count() + ")";
        }
    }

    #region Shared methods

    /// <summary>
    /// Contains methods and properties that helps to manage and gather <see cref="Assembly"/> information
    /// </summary>
    public static partial class Assemblies
    {
#if NETCORE || WINDOWS_UWP || ANDROID

        /// <summary>
        /// Gets a value that determines if the Debugger is attached to the process
        /// </summary>
        public static bool IsDebugging { get { return Debugger.IsAttached; } }

#else

        /// <summary>
        /// Gets a value that determines if the <see cref="Assembly.GetEntryAssembly"/> or <see cref="Assembly.GetCallingAssembly"/> is in debug mode
        /// </summary>
        public static bool IsDebugging
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();

                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                var attrib = assembly.GetCustomAttribute<DebuggableAttribute>();
                return attrib == null ? false : attrib.IsJITTrackingEnabled;
            }
        }

#endif
    }

    /// <summary>
    /// Contains methods and properties that helps to manage and gather <see cref="Assembly"/> information
    /// </summary>
    public static partial class Assemblies
    {
#if WINDOWS_UWP || NETCORE

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
            var types = FilterTypes(assembly.DefinedTypes);

            var definedTypes = types.Select(x => new TypesWithImplementedInterfaces
            {
                interfaces = x.ImplementedInterfaces.ToArray(),
                typeInfo = x
            });
            typesWithImplementedInterfaces.AddRange(definedTypes);
            AssemblyAndResourceNamesInfo.AddRange(assembly.GetManifestResourceNames().Select(x => new AssemblyAndResourceNameInfo(assembly, x)));

            LoadedAssemblyChanged?.Invoke(null, new AssemblyAddedEventArgs(assembly, types.Select(x => x.AsType()).ToArray()));
        }

#endif
    }

    /// <summary>
    /// Contains methods and properties that helps to manage and gather <see cref="Assembly"/> information
    /// </summary>
    public static partial class Assemblies
    {
#if NETCORE

        private static Assembly ResolveAssembly(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            Output.WriteLineInfo($"Requesting Resolving of Assembly '{assemblyName.FullName}'");

            var assembly = _assemblies.FirstOrDefault(x =>
                x.FullName.Equals(assemblyName.FullName, StringComparison.CurrentCultureIgnoreCase) || assemblyName.Name.StartsWith(x.GetName().Name, StringComparison.CurrentCultureIgnoreCase));

            if (assembly == null)
            {
                var runtime = DependencyContext.Default.RuntimeLibraries.FirstOrDefault(x => x.Name.Equals(assemblyName.Name, StringComparison.CurrentCultureIgnoreCase));
                if (runtime != null)
                    return context.LoadFromAssemblyPath(runtime.Path);
            }

            if (assembly == null)
            {
                assembly = context.LoadFromAssemblyName(assemblyName);
                if (assembly != null)
                    return assembly;
            }

            // The following resolve tries can only be successfull if the dll's name is the same as the simple Assembly name
            // Try to load it from application directory
            var file = Path.Combine(ApplicationInfo.ApplicationPath.FullName, $"{assemblyName.Name}.dll");
            if (File.Exists(file))
                return context.LoadFromAssemblyPath(file);

            // Try to load it from current domain's base directory
            file = Path.Combine(AppContext.BaseDirectory, $"{assemblyName.Name}.dll");
            if (File.Exists(file))
                return context.LoadFromAssemblyPath(file);

            return assembly;
        }

#elif !WINDOWS_UWP

        private static Assembly ResolveAssembly(object sender, ResolveEventArgs e)
        {
            if (e.RequestingAssembly == null)
                Output.WriteLineInfo($"Assembly requesting for '{e.Name}'");
            else
                Output.WriteLineInfo($"Assembly '{e.RequestingAssembly.FullName}' requesting for '{e.Name}'");

            var assembly = _assemblies.FirstOrDefault(x => x.FullName == e.Name || e.Name.StartsWith(x.GetName().Name));

            // The following resolve tries can only be successfull if the dll's name is the same as the simple Assembly name

            // Try to load it from application directory
            if (assembly == null)
            {
                var file = Path.Combine(ApplicationInfo.ApplicationPath.FullName, $"{new AssemblyName(e.Name).Name}.dll");
                if (File.Exists(file))
                    return Assembly.LoadFile(file);
            }

            // Try to load it from current domain's base directory
            var assemblyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{new AssemblyName(e.Name).Name}.dll");
            if (File.Exists(assemblyFile))
                return Assembly.LoadFile(assemblyFile);

            // As last resort try to load it from the Cauldron.Core.dlls directory
            assemblyFile = Path.Combine(Path.GetDirectoryName(typeof(Assemblies).Assembly.Location), $"{new AssemblyName(e.Name).Name}.dll");
            if (File.Exists(assemblyFile))
                return Assembly.LoadFile(assemblyFile);

            return assembly;
        }

#endif
    }

    /// <summary>
    /// Contains methods and properties that helps to manage and gather <see cref="Assembly"/> information
    /// </summary>
    public static partial class Assemblies
    {
#if NETCORE
        // http://www.michael-whelan.net/replacing-appdomain-in-dotnet-core/

        private static IEnumerable<Assembly> GetAssemblies()
        {
            var entryAssemblyName = Assembly.GetEntryAssembly().GetName();
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;

            foreach (var library in dependencies)
                if (IsCandidateLibrary(library, entryAssemblyName.Name))
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }

            return assemblies;
        }

        private static bool IsCandidateLibrary(RuntimeLibrary library, string assemblyName) => library.Name == assemblyName || library.Dependencies.Any(x => x.Name.StartsWith(assemblyName));

#endif
    }

    /// <summary>
    /// Contains methods and properties that helps to manage and gather <see cref="Assembly"/> information
    /// </summary>
    public static partial class Assemblies
    {
#if ANDROID || NETCORE || !WINDOWS_UWP

        /// <summary>
        /// Loads the contents of all assemblies that matches the specified filter
        /// </summary>
        /// <param name="directory">The directory where the assemblies are located</param>
        /// <param name="filter">
        /// The search string to match against the names of files in <paramref name="directory"/>. This parameter can contain a combination of
        /// valid literal path and wildcard (* and ?) characters, but doesn't support regular expressions.
        /// </param>
        /// <exception cref="FileLoadException">A file that was found could not be loaded</exception>
        public static void LoadAssembly(DirectoryInfo directory, string filter = "*.dll")
        {
            foreach (var files in directory.GetFiles(filter))
                LoadAssembly(files);
        }

        /// <summary>
        /// Loads the contents of an assembly file on the specified path.
        /// </summary>
        /// <param name="fileInfo">The path of filename of the assembly</param>
        /// <exception cref="NotSupportedException">The <paramref name="fileInfo"/> is a dynamic assembly.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="fileInfo"/> parameter is null.</exception>
        /// <exception cref="FileLoadException">A file that was found could not be loaded</exception>
        /// <exception cref="FileNotFoundException">The <paramref name="fileInfo"/> does not exist</exception>
        /// <exception cref="BadImageFormatException"><paramref name="fileInfo"/> is not a valid assembly.</exception>
        public static void LoadAssembly(FileInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException(nameof(fileInfo));

            if (!fileInfo.Exists)
                throw new FileNotFoundException($"The file '{fileInfo.FullName}' does not exist");

#if NETCORE
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(fileInfo.FullName);
#else
            var assembly = Assembly.LoadFile(fileInfo.FullName);
#endif

            if (assembly.IsDynamic)
                throw new NotSupportedException($"Dynamic assemblies are not supported.");

            if (Assemblies.Known.Any(x => x.ManifestModule.Name == assembly.ManifestModule.Name))
                return; // this is already loaded... No need to load again

            Assemblies.Known.Add(assembly);
            var types = FilterTypes(assembly.DefinedTypes).ToArray();

            var definedTypes = types.Select(x => new TypesWithImplementedInterfaces
            {
                interfaces = x.ImplementedInterfaces.ToArray(),
                typeInfo = x
            });

            typesWithImplementedInterfaces.AddRange(definedTypes);
            AssemblyAndResourceNamesInfo.AddRange(assembly.GetManifestResourceNames().Select(x => new AssemblyAndResourceNameInfo(assembly, x)));

#if NETCORE
            LoadedAssemblyChanged?.Invoke(null, new AssemblyAddedEventArgs(assembly, types.Select(x => x.AsType()).ToArray()));
#else
            LoadedAssemblyChanged?.Invoke(null, new AssemblyAddedEventArgs(assembly, types));
#endif
        }

#endif
    }

    #endregion Shared methods
}