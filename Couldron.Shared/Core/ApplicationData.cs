using System;
using System.IO;

namespace Couldron.Core
{
    /// <summary>
    /// Provides access to the application data store
    /// </summary>
    public sealed class ApplicationData
    {
        private static volatile ApplicationData instance;
        private static object syncRoot = new object();

        private ApplicationData()
        {
            this.RoamingFolder = this.GetOrCreate(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            this.LocalFolder = this.GetOrCreate(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            this.TemporaryFolder = this.GetOrCreate(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp"));
        }

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
        public string LocalFolder { get; private set; }

        /// <summary>
        /// Gets the root folder in the roaming app data store.
        /// </summary>
        public string RoamingFolder { get; private set; }

        /// <summary>
        /// Gets the root folder in the temporary app data store.
        /// </summary>
        public string TemporaryFolder { get; private set; }

        private string GetOrCreate(string folder)
        {
            var path = Path.Combine(folder, Utils.ApplicationPublisher, Utils.ApplicationName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}