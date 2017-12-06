using Cauldron.Activator;
using Cauldron.Core.Extensions;
using Cauldron.XAML;
using Cauldron.XAML.Theme;
using EveMarket.ViewModels;
using EveOnlineApi;
using System;
using System.Threading.Tasks;

namespace EveMarket
{
    public sealed class App : ApplicationBase
    {
        public App()
        {
            this.UrlProtocolNames = new string[] { "eveonline1" };
            this.IsSinglePage = true;
            this.StartSearchCommand = new RelayCommand(this.StartSearchAction);
        }

        public string SearchedItem { get; set; }

        public IRelayCommand StartSearchCommand { get; private set; }

        public override async void OnException(Exception e) =>
            await this.MessageDialog.ShowException(e);

        protected override async void OnActivationProtocol(Uri uri) =>
            await this.Navigator.NavigateAsync<ItemInfoViewModel>(uri.AbsolutePath.ToLong());

        protected override async Task OnPreload()
        {
            var eveApi = Factory.Create<IEveApi>();
            await eveApi.StaticData.UpdateStaticDataAsync();
            await eveApi.CachePriceAsync();
        }

        protected override void OnResourceLoad()
        {
            CauldronTheme.SetAccentColor(System.Windows.Media.Colors.Brown);
            base.OnResourceLoad();
        }

        protected override async Task OnStartup(LaunchActivatedEventArgs e) => await this.Navigator.NavigateAsync<HomeViewModel>("Typhoon");

        private async void StartSearchAction() =>
            await this.Navigator.NavigateAsync<HomeViewModel>(this.SearchedItem);
    }
}