using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

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
                var propertyInfo = Package.Current.GetType().GetProperty("DisplayName");

                if (propertyInfo == null)
                    return GetApplicationDisplayNameFromManifestAsync().Result;

                return propertyInfo.GetValue(Package.Current) as string;
            }
        }

        /// <summary>
        /// Gets teh application publisher name
        /// </summary>
        public static string ApplicationPublisher
        {
            get
            {
                return Package.Current.Id.Publisher;
            }
        }

        private static async Task<string> GetApplicationDisplayNameFromManifestAsync()
        {
            var xml = new Windows.Data.Xml.Dom.XmlDocument();
            var file = await Package.Current.InstalledLocation.GetFileAsync("AppxManifest.xml");
            xml.LoadXml(await FileIO.ReadTextAsync(file));

            return xml.GetElementsByTagName("DisplayName")[0].InnerText.Trim();
        }
    }
}