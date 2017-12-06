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

namespace Cauldron.Core
{
    /// <summary>
    /// Provides methods to retrieve information about the application
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
                Func<Task<string>> getApplicationDisplayNameFromManifestAsync = async () =>
                 {
                     var xml = new Windows.Data.Xml.Dom.XmlDocument();
                     var file = await Package.Current.InstalledLocation.GetFileAsync("AppxManifest.xml");
                     xml.LoadXml(await FileIO.ReadTextAsync(file));

                     return xml.GetElementsByTagName("DisplayName")[0].InnerText.Trim();
                 };

                var result = Package.Current.DisplayName;

                if (string.IsNullOrEmpty(result))
                    return AsyncHelper.RunSync(() => getApplicationDisplayNameFromManifestAsync());

                return result;
#else

                var assembly = Assembly.GetEntryAssembly();
#if !NETCORE
                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();
#endif
                return assembly.GetName().Name;
#endif
            }
        }

        /// <summary>
        /// Gets the full path of the application
        /// </summary>
#if WINDOWS_UWP

        public static StorageFolder ApplicationPath
        {
            get { return Package.Current.InstalledLocation; }
        }

#else

        public static DirectoryInfo ApplicationPath
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();
#if !NETCORE
                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();
#endif

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
#if !NETCORE
                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();
#endif

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
#if !NETCORE
                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();
#endif

                return assembly.GetName().Version;
#endif
            }
        }

#if !WINDOWS_UWP

        /// <summary>
        /// Gets the application description
        /// </summary>
        public static string Description
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();
#if !NETCORE
                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();
#endif

                return assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
            }
        }

#endif

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
#if !NETCORE
                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();
#endif

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
#if !NETCORE
                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();
#endif

                return assembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkDisplayName;
#endif
            }
        }
    }
}