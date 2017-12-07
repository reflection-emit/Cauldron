using Cauldron.Core.Reflection;
using Cauldron.Localization;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Configuration.Install;
using System.Reflection;
using System.Security;
using System.ServiceProcess;

namespace Cauldron.WindowsService
{
    /// <summary>
    /// Provides a base class for the windows service installer.
    /// </summary>
    public abstract class WindowsServiceInstaller : Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;

        /// <summary>
        /// Initializes a new instance of <see cref="WindowsServiceInstaller"/>.
        /// </summary>
        protected WindowsServiceInstaller()
        {
            // Get and deserialize the json with all the information we need to install the service
            var configJson = JsonConvert.DeserializeObject<Configuration>(
                Assemblies.GetManifestResource(x => x.Filename.EndsWith("configuration.json", StringComparison.InvariantCultureIgnoreCase) && x.Assembly == Assembly.GetEntryAssembly())
                .TryEncode());

            this.serviceProcessInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            serviceProcessInstaller.Account = configJson.Account;

            serviceInstaller.DisplayName = Locale.Current[configJson.DisplayName];
            serviceInstaller.Description = Locale.Current[configJson.Description];
            serviceInstaller.StartType = configJson.StartType;
            serviceInstaller.DelayedAutoStart = configJson.DelayedAutoStart;
            serviceInstaller.ServiceName = configJson.ServiceName;

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);

            this.Committed += (s, e) =>
            {
                serviceInstaller.SetServiceFailureActions(new FailureActions
                {
                    FirstFailure = configJson.FirstFailure,
                    SecondFailure = configJson.SecondFailure,
                    SubsequentFailure = configJson.SubsequentFailure,
                    ResetFailCountAfter = TimeSpan.FromDays(configJson.ResetFailCountAfter),
                    RestartServiceAfter = TimeSpan.FromMinutes(configJson.RestartServiceAfter),
                    RunProgram = configJson.RunProgram.ToFileInfo().GetShortPath(),
                    RunProgramArguments = configJson.RunProgramArguments
                });
                serviceInstaller.TryStartService();
            };
        }

        /// <summary>
        /// Gets or sets the password of the <see cref="UserName"/>.
        /// <para/>
        /// This is only used if the account that starts the service is 'User' or 'NetworkService'.
        /// </summary>
        public SecureString Password
        {
            get { return this.serviceProcessInstaller.Password.ToSecureString(); }
            set { this.serviceProcessInstaller.Password = value.GetString(); }
        }

        /// <summary>
        /// Gets or sets the username that used to start the service.
        /// <para/>
        /// This is only used if the account that starts the service is 'User' or 'NetworkService'.
        /// </summary>
        public string UserName
        {
            get { return this.serviceProcessInstaller.Username; }
            set { this.serviceProcessInstaller.Username = value; }
        }

        /// <summary>
        /// Occures if the service is installed.
        /// </summary>
        public void Install()
        {
            var transactedInstaller = new TransactedInstaller();
            transactedInstaller.Installers.Add(this);

            var path = $"/assemblypath={this.GetType().Assembly.Location}";

            var context = new InstallContext("", new string[] { path });
            transactedInstaller.Context = context;
            transactedInstaller.Install(new Hashtable());
        }

        /// <summary>
        /// Occures if the service is uninstalled.
        /// </summary>
        public void Uninstall()
        {
            var transactedInstaller = new TransactedInstaller();
            transactedInstaller.Installers.Add(this);

            var path = $"/assemblypath={this.GetType().Assembly.Location}";

            var context = new InstallContext("", new string[] { path });
            transactedInstaller.Context = context;
            transactedInstaller.Uninstall(null);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the System.ComponentModel.Component and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                this.serviceProcessInstaller?.Dispose();

            base.Dispose(disposing);
        }
    }
}