using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cauldron.Core.Reflection
{
    using Cauldron.Core.Diagnostics;

    /// <summary>
    /// Contains methods and properties that helps to manage and gather <see cref="Assembly"/> information
    /// </summary>
    public static partial class Assemblies
    {
        private const string CauldronClassName = "CauldronInterceptionHelper";

        private static ConcurrentBag<Assembly> _assemblies = new ConcurrentBag<Assembly>();
        private static ConcurrentBag<AssemblyResource> _assemblyAndResourceNamesInfo = new ConcurrentBag<AssemblyResource>();
        private static ConcurrentBag<object> _cauldron = new ConcurrentBag<object>();

        static Assemblies()
        {
            try
            {
                GetAllAssemblies();
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
        public static AssemblyResource[] AssemblyAndResourceNamesInfo => _assemblyAndResourceNamesInfo.ToArray();

        /// <summary>
        /// Gets an array of cauldron cache objects
        /// </summary>
        public static object[] CauldronObjects => _cauldron.ToArray();

        /// <summary>
        /// Gets the process executable in the default application domain. In other application
        /// domains, this is the first executable that was executed.
        /// </summary>
        public static Assembly EntryAssembly
        {
            get
            {
                if (AssembliesCore._entryAssembly != null)
                    return AssembliesCore._entryAssembly;

#if !NETFX_CORE

                var assembly = Assembly.GetEntryAssembly();

                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();

                return assembly;
#else
                return null;
#endif
            }
        }

        /// <summary>
        /// Gets a collection of exported types found in the AppDomain
        /// </summary>
        public static IEnumerable<Type> ExportedTypes => _assemblies.SelectMany(x => x.ExportedTypes);

        /// <summary>
        /// Gets an array of <see cref="Assembly"/> that is loaded to the AppDomain
        /// </summary>
        public static Assembly[] Known => _assemblies.ToArray();

        /// <summary>
        /// Adds a new Assembly to the assembly collection
        /// </summary>
        /// <param name="assembly">The assembly to be added</param>
        /// <exception cref="NotSupportedException">
        /// <paramref name="assembly"/> is a dynamic assembly
        /// </exception>
        public static void AddAssembly(Assembly assembly) => AddAssembly(assembly, true);

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

        private static Assembly AddAssembly(Assembly assembly, bool triggerEvent)
        {
            try
            {
                if (assembly.IsDynamic)
                {
                    Debug.WriteLine("Dynamic assemblies are not supported");
                    return null;
                }

                if (_assemblies.Contains(assembly))
                    return null;

                _assemblies.Add(assembly);

                var manifestResourceNames = assembly.GetManifestResourceNames();
                for (int i = 0; i < manifestResourceNames.Length; i++)
                {
                    var resource = new AssemblyResource(assembly, manifestResourceNames[i]);
                    _assemblyAndResourceNamesInfo.Add(resource);
                }

                var cauldronClass = assembly.GetType(CauldronClassName, false, false);
                if (cauldronClass != null)
                {
                    var cauldronObject = Activator.CreateInstance(cauldronClass);
                    _cauldron.Add(cauldronObject);

                    if (triggerEvent)
                        LoadedAssemblyChanged?.Invoke(null, new AssemblyAddedEventArgs(assembly, cauldronObject));
                }

                return assembly;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error while loading an assembly" + e.Message);
            }

            return null;
        }

        private static void LoadAssembly(AssemblyName assemblyName)
        {
            // We have to do this one by one instead of linq in order to be able to react
            // to loading errors
            try
            {
                AddAssembly(Assembly.Load(assemblyName));
            }
            catch (Exception e)
            {
                var message = "Error while loading an assembly" + e.Message;
                Debug.WriteLine(message);
            }
        }
    }
}