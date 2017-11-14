using Cauldron.Core.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

#if NETFX_CORE

using Windows.UI.Xaml;

#elif NETCORE

using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

#endif

namespace Cauldron.Core.Reflection
{
    /// <summary>
    /// Contains methods and properties that helps to manage and gather <see cref="Assembly"/> information
    /// </summary>
    public static partial class Assemblies
    {
        private const string CauldronClassName = "<Cauldron>";

        private static ConcurrentBag<Assembly> _assemblies;
        private static ConcurrentBag<AssemblyResource> _assemblyAndResourceNamesInfo = new ConcurrentBag<AssemblyResource>();
        private static ConcurrentBag<object> _cauldron = new ConcurrentBag<object>();

        static Assemblies()
        {
            try
            {
                GetAllAssemblies();
                GetAllCauldronCache();
                GetAllAssemblyAndResourceNameInfo();
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetStackTrace());

                throw;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine("This may caused by an invalid assembly or incorrect / mismatching target CPUs");

                throw;
            }
        }

        /// <summary>
        /// Occures if the assembly dictionary has changed
        /// </summary>
        public static event EventHandler<AssemblyAddedEventArgs> LoadedAssemblyChanged;

        /// <summary>
        /// Gets an array of <see cref="AssemblyResource"/> that contains all fully qualified
        /// filename of embedded resources and thier corresponding <see cref="Assembly"/>
        /// </summary>
        public static AssemblyResource[] AssemblyAndResourceNamesInfo { get { return _assemblyAndResourceNamesInfo.ToArray(); } }

        /// <summary>
        /// Gets an array of cauldron cache objects
        /// </summary>
        public static object[] CauldronObjects { get { return _cauldron.ToArray(); } }

        /// <summary>
        /// Gets a collection of classes loaded to the AppDomain
        /// </summary>
        public static IEnumerable<Type> Classes
        {
            get
            {
#if NETFX_CORE || NETCORE
                return _assemblies.SelectMany(x => x.ExportedTypes).Where(x => x.GetTypeInfo().IsClass);
#else
                return _assemblies.SelectMany(x => x.ExportedTypes).Where(x => x.IsClass);
#endif
            }
        }

        /// <summary>
        /// Gets a collection of exported types found in the AppDomain
        /// </summary>
        public static IEnumerable<Type> ExportedTypes { get { return _assemblies.SelectMany(x => x.ExportedTypes); } }

        /// <summary>
        /// Gets a colleciton of Interfaces found in the AppDomain
        /// </summary>
        public static IEnumerable<Type> Interfaces
        {
            get
            {
#if NETFX_CORE || NETCORE
                return _assemblies.SelectMany(x => x.ExportedTypes).Where(x => x.GetTypeInfo().IsInterface);
#else
                return _assemblies.SelectMany(x => x.ExportedTypes).Where(x => x.IsInterface);
#endif
            }
        }

        /// <summary>
        /// Gets an array of <see cref="Assembly"/> that is loaded to the AppDomain
        /// </summary>
        public static Assembly[] Known { get { return _assemblies.ToArray(); } }

        /// <summary>
        /// Adds a new Assembly to the assembly collection
        /// </summary>
        /// <param name="assembly">The assembly to be added</param>
        /// <exception cref="NotSupportedException">
        /// <paramref name="assembly"/> is a dynamic assembly
        /// </exception>
        public static void AddAssembly(Assembly assembly)
        {
            if (assembly.IsDynamic)
                throw new NotSupportedException("Dynamic assemblies are not supported");

            if (_assemblies.Any(x => x.ManifestModule.Name.GetHashCode() == assembly.ManifestModule.Name.GetHashCode() && x.ManifestModule.Name == assembly.ManifestModule.Name))
                return;

            _assemblies.Add(assembly);
            object cauldronObject = null;

            foreach (var item in assembly.DefinedTypes)
                if (item.FullName != null && item.FullName.GetHashCode() == CauldronClassName.GetHashCode() && item.FullName == CauldronClassName)
                {
#if NETFX_CORE || NETCORE
                    cauldronObject = Activator.CreateInstance(item.AsType());
#else
                    cauldronObject = Activator.CreateInstance(item);
#endif
                    _cauldron.Add(cauldronObject);
                }

            foreach (var resource in assembly.GetManifestResourceNames().Select(x => new AssemblyResource(assembly, x)))
                _assemblyAndResourceNamesInfo.Add(resource);

            LoadedAssemblyChanged?.Invoke(null, new AssemblyAddedEventArgs(assembly, cauldronObject));
        }

        /// <summary>
        /// Returns the first found <see cref="Assembly"/> that contains an embedded resource with
        /// the given resource name
        /// </summary>
        /// <param name="resourceInfoName">
        /// The end of the fully qualified name of the embedded resource
        /// </param>
        /// <returns>The first found <see cref="Assembly"/> otherwise returns null</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resourceInfoName"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="resourceInfoName"/> parameter is an empty string ("").
        /// </exception>
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
        /// <param name="resourceInfoName">
        /// The end of the fully qualified name of the embedded resource
        /// </param>
        /// <returns>
        /// The manifest resource; or null if no resources were specified during compilation or if
        /// the resource is not visible to the caller.
        /// </returns>
        /// <exception cref="ArgumentNullException">The resourceInfoName parameter is null</exception>
        /// <exception cref="ArgumentException">
        /// The resourceInfoName parameter is an empty string
        /// </exception>
        /// <exception cref="FileLoadException">A file that was found could not be loaded.</exception>
        /// <exception cref="FileNotFoundException">
        /// <paramref name="resourceInfoName"/> was not found.
        /// </exception>
        /// <exception cref="BadImageFormatException">
        /// <paramref name="resourceInfoName"/> is not a valid assembly.
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// Resource length is greater than <see cref="Int64.MaxValue"/>
        /// </exception>
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
        /// <returns>
        /// The manifest resource; or null if no resources were specified during compilation or if
        /// the resource is not visible to the caller.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="selector"/> parameter is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="selector"/> parameter is an empty string
        /// </exception>
        /// <exception cref="FileLoadException">A file that was found could not be loaded.</exception>
        /// <exception cref="NotImplementedException">
        /// Resource length is greater than <see cref="Int64.MaxValue"/>
        /// </exception>
        public static byte[] GetManifestResource(Func<AssemblyResource, bool> selector)
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
        /// <param name="resourceInfoName">
        /// The end of the fully qualified name of the embedded resource
        /// </param>
        /// <returns>
        /// An object that is populated with information about the resource's topology, or null if
        /// the resource is not found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resourceInfoName"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="resourceInfoName"/> parameter is an empty string ("").
        /// </exception>
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
        /// <param name="resourceInfoName">
        /// The end of the fully qualified name of the embedded resource
        /// </param>
        /// <returns>
        /// An <see cref="List{T}"/> object that is populated with information about the resource's topology
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resourceInfoName"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="resourceInfoName"/> parameter is an empty string ("").
        /// </exception>
        public static List<AssemblyResource> GetManifestResources(string resourceInfoName)
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
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="typeName"/> parameter is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="typeName"/> parameter is an empty string
        /// </exception>
        public static Type GetTypeFromName(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));

            if (typeName.Length == 0)
                throw new ArgumentException("The parameter is an empty string", nameof(typeName));

            var result = ExportedTypes
                .FirstOrDefault(x =>
                    x.FullName != null &&
                    x.FullName.GetHashCode() == typeName.GetHashCode() &&
                    x.FullName == typeName);

            if (result != null)
                return result;

            // Maybe the type is just a part of the type name...
            result = ExportedTypes.FirstOrDefault(x => x.FullName != null && x.Name.EndsWith(typeName));

            if (result != null)
                return result;

            // Else let us loop through all known assemblies and look for the type
            foreach (var assembly in _assemblies)
            {
                var type = assembly.GetType(typeName);

                if (type != null)
                    return type;
            }

            // The last resort
            result = Type.GetType(typeName);

            if (result != null)
                return result;

            Debug.WriteLine($"The type '{typeName}' defined by the parameter 'typeName' was not found");
            return null;
        }

        private static void GetAllAssemblies()
        {
#if WINDOWS_UWP || NETCORE
            var assemblies = new List<Assembly>();
            var cauldron = Activator.CreateInstance(AssembliesCORE.EntryAssembly.GetType("<Cauldron>")) as ILoadedAssemblies;
            assemblies.Add(AssembliesCORE.EntryAssembly);

            if (cauldron != null)
                assemblies.AddRange(cauldron.ReferencedAssemblies());
#else

            // Get all assemblies in AppDomain and add them to our list TODO - This will not work in
            // UWP and Core if compiled to native code
            var assemblies = new List<Assembly>();
#endif

#if NETCORE
            AssemblyLoadContext.Default.Resolving += ResolveAssembly;
#elif ANDROID
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
#elif DESKTOP || NETSTANDARD2_0
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ResolveAssembly;
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
#endif

#if DESKTOP || ANDROID || NETCORE || NETSTANDARD2_0
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
                        // We have to do this one by one instead of linq in order to be able to react
                        // to loading errors
                        try
                        {
                            assemblies.Add(Assembly.Load(referencedAssembly));
                        }
                        catch (Exception e)
                        {
                            var message = "Error while loading an assembly" + e.Message;
                            Console.WriteLine(message);
                            Debug.WriteLine(message);
                        }
                    }
                }
                catch (Exception e)
                {
                    var message = "Error while loading an assembly" + e.Message;
                    Console.WriteLine(message);
                    Debug.WriteLine(message);
                }
            }

#endif
            _assemblies = new ConcurrentBag<Assembly>(assemblies.Where(x => !x.IsDynamic).Distinct());
        }

        private static void GetAllAssemblyAndResourceNameInfo()
        {
            var list = new List<AssemblyResource>();

            foreach (var assembly in _assemblies)
            {
                var resources = assembly.GetManifestResourceNames().Select(x => new AssemblyResource(assembly, x));

                if (resources != null && resources.Count() > 0)
                    list.AddRange(resources);
            }

            _assemblyAndResourceNamesInfo = new ConcurrentBag<AssemblyResource>(list.OrderBy(x => x.Filename));
        }

        private static void GetAllCauldronCache()
        {
            foreach (var assembly in _assemblies)
            {
                foreach (var item in assembly.DefinedTypes)
                    if (item.FullName != null && item.FullName.GetHashCode() == CauldronClassName.GetHashCode() && item.FullName == CauldronClassName)
#if NETFX_CORE || NETCORE
                        _cauldron.Add(Activator.CreateInstance(item.AsType()));
#else
                        _cauldron.Add(Activator.CreateInstance(item));
#endif
            }
        }
    }

    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AssembliesCORE
    {
        /*
         * We have to include this type for mixed .net frameworks...
         * NETCore using Desktop dlls
         */

#if  NETFX_CORE || NETCORE
        private static Assembly _entryAssembly;
#endif

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Assembly EntryAssembly
        {
#if  NETFX_CORE || NETCORE
            get { return _entryAssembly; }
            set
            {
                if (_entryAssembly == null)
                    _entryAssembly = value;
            }
#else
            get { return Assembly.GetEntryAssembly(); }
            set { }
#endif
        }
    }

    /// <summary>
    /// Contains data of the <see cref="Assemblies.LoadedAssemblyChanged"/> event.
    /// </summary>
    public sealed class AssemblyAddedEventArgs : EventArgs
    {
        internal AssemblyAddedEventArgs(Assembly assembly, object cauldron)
        {
            this.Cauldron = cauldron;
            this.Assembly = assembly;
        }

        /// <summary>
        /// Gets the assembly that has been added to the known assembly collection
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Gets the auto-generated cauldron object.
        /// </summary>
        public object Cauldron { get; private set; }
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
        /// Gets a value that determines if the <see cref="Assembly.GetEntryAssembly"/> or <see
        /// cref="Assembly.GetCallingAssembly"/> is in debug mode
        /// </summary>
        public static bool IsDebugging
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();

                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                var attrib = assembly.GetCustomAttribute<System.Diagnostics.DebuggableAttribute>();
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
#if NETCORE

        private static Assembly ResolveAssembly(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            Output.WriteLineInfo($"Requesting Resolving of Assembly '{assemblyName.FullName}'");

            var assembly = _assemblies.FirstOrDefault(x => x.FullName.GetHashCode() == assemblyName.FullName.GetHashCode() && x.FullName == assemblyName.FullName);

            if (assembly == null)
            {
                var runtime = DependencyContext.Default.RuntimeLibraries.FirstOrDefault(x => x.Name.GetHashCode() == assemblyName.Name.GetHashCode() && x.Name == assemblyName.Name);
                if (runtime != null)
                    return context.LoadFromAssemblyPath(runtime.Path);
            }

            // The following resolve tries can only be successfull if the dll's name is the same as
            // the simple Assembly name Try to load it from application directory
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
                Debug.WriteLine($"Assembly requesting for '{e.Name}'");
            else
                Debug.WriteLine($"Assembly '{e.RequestingAssembly.FullName}' requesting for '{e.Name}'");

            var assembly = _assemblies.FirstOrDefault(x => x.FullName.GetHashCode() == e.Name.GetHashCode() && x.FullName == e.Name);

            // The following resolve tries can only be successfull if the dll's name is the same as
            // the simple Assembly name

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
                LoadAssembly(files[i]);

            LoadedAssemblyChanged?.Invoke(null, new AssemblyAddedEventArgs(null, null));
        }

        /// <summary>
        /// Loads the contents of an assembly file on the specified path.
        /// </summary>
        /// <param name="fileInfo">The path of filename of the assembly</param>
        /// <exception cref="NotSupportedException">
        /// The <paramref name="fileInfo"/> is a dynamic assembly.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="fileInfo"/> parameter is null.
        /// </exception>
        /// <exception cref="FileLoadException">A file that was found could not be loaded</exception>
        /// <exception cref="FileNotFoundException">
        /// The <paramref name="fileInfo"/> does not exist
        /// </exception>
        /// <exception cref="BadImageFormatException">
        /// <paramref name="fileInfo"/> is not a valid assembly.
        /// </exception>
        public static void LoadAssembly(FileInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException(nameof(fileInfo));

            if (!fileInfo.Exists)
                throw new FileNotFoundException($"The file '{fileInfo.FullName}' does not exist");

            try
            {
#if NETCORE
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(fileInfo.FullName);
#else
                var assembly = Assembly.LoadFile(fileInfo.FullName);
#endif

                if (assembly.IsDynamic)
                    throw new NotSupportedException($"Dynamic assemblies are not supported.");

                AddAssembly(assembly);
            }
            catch (ReflectionTypeLoadException e)
            {
                throw new Exception("Unable to load one or more of the requested types. Retrieve the LoaderExceptions property for more information.\r\n" + fileInfo.FullName, e);
            }
        }

#endif
    }

    #endregion Shared methods
}