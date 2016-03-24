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
        private static ConcurrentList<TypesWithImplementedInterfaces> typesWithImplementedInterfaces;

        static AssemblyUtil()
        {
            GetAllAssemblies();
            GetAllDefinedTypes();
            GetAllAssemblyAndResourceNameInfo();
        }

        /// <summary>
        /// Gets a collection of <see cref="Assembly"/> that is loaded to the <see cref="AssemblyUtil"/>
        /// </summary>
        public static ConcurrentList<Assembly> Assemblies { get; private set; }

        /// <summary>
        /// Gets a collection of <see cref="AssemblyAndResourceNameInfo"/> that contains all fully qualified filename of embedded resources and thier corresponding <see cref="Assembly"/>
        /// </summary>
        public static ConcurrentList<AssemblyAndResourceNameInfo> AssemblyAndResourceNamesInfo { get; private set; }

        /// <summary>
        /// Gets a collection of classes loaded to the <see cref="AssemblyUtil"/>
        /// </summary>
        public static IEnumerable<TypeInfo> Classes { get { return DefinedTypes.Where(x => x.IsClass); } }

        /// <summary>
        /// Gets a colleciton of Types found in the loaded <see cref="Assembly"/>
        /// </summary>
        public static IEnumerable<TypeInfo> DefinedTypes { get { return typesWithImplementedInterfaces.Select(x => x.typeInfo); } }

        /// <summary>
        /// Gets a colleciton of Interfaces found in the loaded <see cref="Assembly"/>
        /// </summary>
        public static IEnumerable<TypeInfo> Interfaces { get { return DefinedTypes.Where(x => x.IsInterface); } }

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
        public static Stream GetManifestResourceStream(string resourceInfoName)
        {
            if (resourceInfoName == null)
                throw new ArgumentNullException(nameof(resourceInfoName));

            if (resourceInfoName.Length == 0)
                throw new ArgumentException("The parameter is an empty string", nameof(resourceInfoName));

            var result = AssemblyAndResourceNamesInfo.FirstOrDefault(x => x.Filename.EndsWith(resourceInfoName, StringComparison.OrdinalIgnoreCase));

            if (result == null)
                throw new FileNotFoundException("resourceInfoName was not found.");

            return result.Assembly.GetManifestResourceStream(result.Filename);
        }

        /// <summary>
        /// Tries to find/identify a <see cref="Type"/> by its name
        /// </summary>
        /// <param name="typeName">The name of the <see cref="Type"/>.
        /// An <see cref="Assembly.FullName"/> can also be given delimeter by an @.
        /// <para/>
        /// Example:
        /// <para/>
        /// <code>
        /// TypeName@AssemblyName
        /// </code>
        /// </param>
        /// <returns>The <see cref="Type"/> that is defined by the parameter <paramref name="typeName"/></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeName"/> parameter is null</exception>
        /// <exception cref="ArgumentException">The <paramref name="typeName"/> parameter is an empty string</exception>
        /// <exception cref="FileNotFoundException"><paramref name="typeName"/> was not found.</exception>
        public static Type GetTypeFromName(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));

            if (typeName.Length == 0)
                throw new ArgumentException("The parameter is an empty string", nameof(typeName));

            TypeInfo result;

            if (typeName.IndexOf('@') >= 0)
            {
                var splitted = typeName.Split('@');
                result = DefinedTypes.FirstOrDefault(x => x.Assembly.FullName.StartsWith(splitted[1]) && x.Name.EndsWith(splitted[0]));
            }
            else
                result = DefinedTypes.FirstOrDefault(x => x.Name.EndsWith(typeName));

            if (result != null)
                return result.AsType();

            throw new FileNotFoundException("The paramater typeName was not found.", typeName);
        }

        /// <summary>
        /// Returns all Types that implements the interface <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The interface <see cref="Type"/></typeparam>
        /// <returns>A colletion of <see cref="Type"/> that implements the interface <typeparamref name="T"/> otherwise null</returns>
        /// <exception cref="ArgumentException"><typeparamref name="T"/> is not an interface</exception>
        public static IEnumerable<TypeInfo> GetTypesImplementsInterface<T>()
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException(string.Format("The Type '{0}' is not an interface.", typeof(T).FullName));

            var interfaceType = typeof(T);
            var result = typesWithImplementedInterfaces.Where(x => x.interfaces.Length > 0 && x.interfaces.Contains(interfaceType));

            if (result == null)
                return null;

            return result.Select(x => x.typeInfo);
        }

        public static void LoadAssembly(string path, bool relativPath = true)
        {
            var assemblyPath = string.Empty;

            if (relativPath)
                assemblyPath = Path.Combine(Utils.ApplicationPath, path);
            else
                assemblyPath = path;

            var assembly = Assembly.LoadFile(assemblyPath);

            typesWithImplementedInterfaces.AddRange(
                assembly.DefinedTypes.Select(x => new TypesWithImplementedInterfaces
                {
                    interfaces = x.ImplementedInterfaces.ToArray(),
                    typeInfo = x
                }));

            AssemblyAndResourceNamesInfo.AddRange(assembly.GetManifestResourceNames().Select(x => new AssemblyAndResourceNameInfo(assembly, x)));
        }

        #region Private Methods

        private static void GetAllAssemblies()
        {
            // Get all assemblies in AppDomain and add them to our list
            // TODO - This will not work in UWP and Core if compiled to native code
            var assemblies = new List<Assembly>();
            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            assemblies.Add(typeof(AssemblyUtil).Assembly);
            Assemblies = new ConcurrentList<Assembly>(assemblies.Distinct());
        }

        private static void GetAllAssemblyAndResourceNameInfo()
        {
            var list = new List<AssemblyAndResourceNameInfo>();

            foreach (var assembly in AssemblyUtil.Assemblies)
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

            foreach (var assembly in AssemblyUtil.Assemblies)
            {
                try
                {
                    types.AddRange(assembly.DefinedTypes);
                }
                catch (Exception e)
                {
                    // Just ignore this exception... Its actually not very interesting and cruicial
                    Debug.WriteLine(e.Message);
                }
            }

            typesWithImplementedInterfaces = new ConcurrentList<TypesWithImplementedInterfaces>(
                types /* Exclude everything that is in the namespace microsoft and system. We dont need those for anything */
                    .Where(x => !(
                        string.IsNullOrEmpty(x.Namespace) || x.FullName.StartsWith("Microsoft.") ||
                        x.FullName.StartsWith("System.") || x.FullName.StartsWith("Windows.") || x.FullName.StartsWith("MS.Internal.") ||
                        x.FullName.StartsWith("<CrtImplementationDetails>") || x.FullName.StartsWith("<CppImplementationDetails>") ||
                        x.FullName.StartsWith("MS.") || x.FullName.StartsWith("Standard.")))
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