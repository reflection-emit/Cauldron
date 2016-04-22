using System.IO;
using System.Reflection;

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
                var entryAssembly = Assembly.GetEntryAssembly();

                if (entryAssembly == null)
                    return Assembly.GetCallingAssembly().GetName().Name;

                return entryAssembly.GetName().Name;
            }
        }

        /// <summary>
        /// Gets the full path of the application
        /// </summary>
        public static string ApplicationPath
        {
            get
            {
                // this will return null in some cases
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly == null)
                    return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

                return Path.GetDirectoryName(entryAssembly.Location);
            }
        }

        /// <summary>
        /// Gets teh application publisher name
        /// </summary>
        public static string ApplicationPublisher
        {
            get
            {
                var company = Assembly.GetCallingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;

                if (string.IsNullOrEmpty(company))
                    return Assembly.GetAssembly(typeof(Utils)).GetCustomAttribute<AssemblyCompanyAttribute>().Company;

                return company;
            }
        }
    }
}