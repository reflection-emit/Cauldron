using System;
using System.Reflection;

#if WINDOWS_UWP

using Windows.ApplicationModel;
using Windows.Storage;
using System.Threading.Tasks;

#else

using System.Runtime.Versioning;
using System.IO;

#endif

namespace Cauldron.Reflection
{
    /// <summary>
    /// Provides methods to retrieve information about the application. In UWP it is a wrapper for Package.Current
    /// </summary>
    public static class ApplicationInfo
    {
        /// <summary>
        /// Gets the application name
        /// </summary>
        public static string ApplicationName
        {
            get
            {
#if WINDOWS_UWP
                return Package.Current.DisplayName;
#else

                var assembly = Assembly.GetEntryAssembly();

                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();

                return assembly.GetName().Name;
#endif
            }
        }

        /// <summary>
        /// Gets the full path of the application
        /// </summary>
#if WINDOWS_UWP

        public static StorageFolder ApplicationPath => Package.Current.InstalledLocation;

#else

        public static DirectoryInfo ApplicationPath
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();

                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();

                return new DirectoryInfo(Path.GetDirectoryName(assembly.Location));
            }
        }

#endif

        /// <summary>
        /// Gets the application publisher name
        /// </summary>
        public static string ApplicationPublisher
        {
            get
            {
#if WINDOWS_UWP
                return Package.Current.PublisherDisplayName;
#else

                var assembly = Assembly.GetEntryAssembly();

                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();

                return assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
#endif
            }
        }

        /// <summary>
        /// Gets a value representing the version of the application
        /// </summary>
        public static Version ApplicationVersion
        {
            get
            {
#if WINDOWS_UWP
                return new Version(Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision);
#else

                var assembly = Assembly.GetEntryAssembly();

                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();

                return assembly.GetName().Version;
#endif
            }
        }

        /// <summary>
        /// Gets the application description
        /// </summary>
        public static string Description
        {
            get
            {
#if WINDOWS_UWP
                return Package.Current.Description;
#else
                var assembly = Assembly.GetEntryAssembly();
                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();

                return assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
#endif
            }
        }

        /// <summary>
        /// Gets the applications product name
        /// </summary>
        public static string ProductName
        {
            get
            {
#if WINDOWS_UWP
                return Package.Current.Id.Name;
#else
                var assembly = Assembly.GetEntryAssembly();

                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();

                return assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
#endif
            }
        }

        /// <summary>
        /// Gets the target framework the application was built against
        /// </summary>
        public static string TargetFramework
        {
            get
            {
#if WINDOWS_UWP
                return "Universal Windows Platform";
#else
                var assembly = Assembly.GetEntryAssembly();

                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();

                return assembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkDisplayName;
#endif
            }
        }
    }
}