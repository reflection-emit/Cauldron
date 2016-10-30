using Cauldron.Core;
using System;
using System.IO;

namespace Windows.Storage /* So that precompiler definitions are not required if classes are shared between UWP and Desktop */
{
    /// <summary>
    /// Provides access to the application data store
    /// </summary>
    public sealed class ApplicationData
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

        /// <summary>
        /// Gets the root folder in the local app data store.
        /// </summary>
        public DirectoryInfo LocalFolder
        {
            get { return this.GetOrCreate(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)); }
        }

        /// <summary>
        /// Gets the root folder in the roaming app data store.
        /// </summary>
        public DirectoryInfo RoamingFolder
        {
            get { return this.GetOrCreate(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)); }
        }

        /// <summary>
        /// Gets the root folder in the temporary app data store.
        /// </summary>
        public DirectoryInfo TemporaryFolder
        {
            get { return this.GetOrCreate(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp")); }
        }

        private DirectoryInfo GetOrCreate(string folder)
        {
            var path = Path.Combine(folder, ApplicationInfo.ApplicationPublisher, ApplicationInfo.ApplicationName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return new DirectoryInfo(path);
        }
    }
}