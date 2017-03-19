using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.XAML;
using Cauldron.XAML.Navigation;
using EveMarket.ViewModels;
using EveOnlineApi;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace EveMarket
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : ApplicationBase
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.StartSearchCommand = new RelayCommand(this.StartSearchAction);

            Assemblies.AddAssembly(Assembly.Load(new AssemblyName("EveOnlineApi")));
            Assemblies.AddAssembly(Assembly.Load(new AssemblyName("EveMarket.Views")));
        }

        public string SearchedItem { get; set; }

        public IRelayCommand StartSearchCommand { get; private set; }

        public override async void OnException(Exception e) =>
            await this.MessageDialog.ShowException(e);

        protected override async void OnActivationProtocol(Uri uri) =>
            await this.Navigator.NavigateAsync<ItemInfoViewModel>(uri.AbsolutePath.ToLong());

        protected override async Task OnPreload()
        {
            this.EnableFrameRateCounter = false;
            var eveApi = Factory.Create<IEveApi>();
            await eveApi.StaticData.UpdateStaticDataAsync();
            await eveApi.CachePriceAsync();
        }

        protected override async Task OnStartup(LaunchActivatedEventArgs e) =>
            await this.Navigator.NavigateAsync<HomeViewModel>("Typhoon");

        private async void StartSearchAction() =>
            await this.Navigator.NavigateAsync<HomeViewModel>(this.SearchedItem);
    }
}