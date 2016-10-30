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
                var entryAssembly = Assembly.GetEntryAssembly();

                if (entryAssembly == null)
                    return Assembly.GetCallingAssembly().GetName().Name;

                return entryAssembly.GetName().Name;
#endif
            }
        }

        /// <summary>
        /// Gets the full path of the application
        /// </summary>
        public static string ApplicationPath
        {
            get
            {
#if WINDOWS_UWP
                return Package.Current.InstalledLocation.Path;
#else
                // this will return null in some cases
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly == null)
                    return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

                return Path.GetDirectoryName(entryAssembly.Location);
#endif
            }
        }

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
                var company = Assembly.GetCallingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;

                if (string.IsNullOrEmpty(company) && Assembly.GetEntryAssembly() != null)
                    company = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;

                if (string.IsNullOrEmpty(company))
                    company = Assembly.GetAssembly(typeof(ApplicationInfo)).GetCustomAttribute<AssemblyCompanyAttribute>().Company;

                return company;
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
                var entryAssembly = Assembly.GetEntryAssembly();

                if (entryAssembly == null)
                    return Assembly.GetCallingAssembly().GetName().Version;

                return entryAssembly.GetName().Version;
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
                var company = Assembly.GetCallingAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;

                if (string.IsNullOrEmpty(company) && Assembly.GetEntryAssembly() != null)
                    company = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;

                if (string.IsNullOrEmpty(company))
                    company = Assembly.GetAssembly(typeof(ApplicationInfo)).GetCustomAttribute<AssemblyProductAttribute>().Product;

                return company;
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
                var entryAssembly = Assembly.GetEntryAssembly();

                if (entryAssembly == null)
                    entryAssembly = Assembly.GetCallingAssembly();

                var attrib = entryAssembly.GetCustomAttribute<TargetFrameworkAttribute>();

                if (attrib == null)
                    return string.Empty;

                return attrib.FrameworkDisplayName;
#endif
            }
        }
    }
}