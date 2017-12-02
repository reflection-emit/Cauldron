using Cauldron.Core.Reflection;
using System;
using System.IO;

#if NETCORE

using System.Runtime.InteropServices;

#endif

namespace Cauldron.Core
{
    /// <summary>
    /// Provides access to the application data store
    /// </summary>
    public sealed class ApplicationData /* This is a wrapper that has almost the same methods and properties as Windows.Storage.ApplicationData */
    {
        private static volatile ApplicationData instance;
        private static object syncRoot = new object();

        /// <summary>
        /// Current instance of the <see cref="ApplicationData"/>
        /// </summary>
        public static ApplicationData Current
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ApplicationData();
                    }
                }

                return instance;
            }
        }

#if NETCORE

        /// <summary>
        /// Gets the root folder in the local app data store.
        /// </summary>
        public DirectoryInfo LocalFolder
        {
            get { return this.GetOrCreate(this.LocalApplicationData); }
        }

        // TODO - Roaming folder for Mac and Linux?
        /*
        /// <summary>
        /// Gets the root folder in the roaming app data store.
        /// </summary>
        public DirectoryInfo RoamingFolder
        {
            get { return this.GetOrCreate(this.LocalApplicationData); }
        }
        */

        /// <summary>
        /// Gets the root folder in the temporary app data store.
        /// </summary>
        public DirectoryInfo TemporaryFolder
        {
            get { return this.GetOrCreate(Path.Combine(this.LocalApplicationData, "Temp")); }
        }

        private string LocalApplicationData
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return Environment.GetEnvironmentVariable("LocalAppData");

                return Environment.GetEnvironmentVariable("Home");
            }
        }

        private DirectoryInfo GetOrCreate(string folder)
        {
            var path = Path.Combine(folder, ApplicationInfo.ApplicationPublisher, ApplicationInfo.ApplicationName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return new DirectoryInfo(path);
        }

#else

        /// <summary>
        /// Gets the root folder in the local app data store.
        /// </summary>
        public DirectoryInfo LocalFolder => this.GetOrCreate(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

        /// <summary>
        /// Gets the root folder in the roaming app data store.
        /// </summary>
        public DirectoryInfo RoamingFolder => this.GetOrCreate(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

        /// <summary>
        /// Gets the root folder in the temporary app data store.
        /// </summary>
        public DirectoryInfo TemporaryFolder => this.GetOrCreate(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp"));

        private DirectoryInfo GetOrCreate(string folder)
        {
            var path = Path.Combine(folder, ApplicationInfo.ApplicationPublisher, ApplicationInfo.ApplicationName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return new DirectoryInfo(path);
        }

#endif
    }
}