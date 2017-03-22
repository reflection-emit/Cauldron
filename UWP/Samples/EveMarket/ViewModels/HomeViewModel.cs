using Cauldron.Activator;
using Cauldron.Core.Extensions;
using Cauldron.XAML;
using Cauldron.XAML.ViewModels;
using EveMarket.Views;
using EveOnlineApi;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EveMarket.ViewModels
{
    [View(typeof(HomeView))]
    public class HomeViewModel : ViewModelBase, IFactoryInitializeComponent
    {
        [Inject]
        private IEveApi eveApi = null;

        private string searchString;

        [ComponentConstructor]
        public HomeViewModel(string searchString)
        {
            this.searchString = searchString;
            this.TypeItemCollection = new ObservableCollection<TypeNameItemViewModel>();
            this.OpenInfoCommand = new RelayCommand<IViewModel>(this.OpenInfoAction);
        }

        public IViewModel Details { get; set; }

        /// <summary>
        /// Gets the OpenInfo command
        /// </summary>
        public ICommand OpenInfoCommand { get; private set; }

        public ObservableCollection<TypeNameItemViewModel> TypeItemCollection { get; private set; }

        public override async void OnException(Exception e)
        {
            await this.MessageDialog.ShowException(e);
        }

        public async void OnInitializeComponent()
        {
            await this.RunDispatcherAsync(async () =>
            {
                if (this.searchString == null || this.searchString.Length < 5)
                {
                    await this.MessageDialog.ShowOKAsync("morethan5CharsRequired");
                    return;
                }

                this.TypeItemCollection.Clear();

                foreach (var item in this.eveApi.StaticData.ItemTypes
                    .Where(x => x.Value.IsPublished &&
                    (x.Value.Name.Contains(this.searchString, StringComparison.CurrentCultureIgnoreCase) ||
                    x.Value.GroupName.Contains(this.searchString, StringComparison.CurrentCultureIgnoreCase)))
                    .OrderBy(x => x.Value.Name))
                {
                    this.TypeItemCollection.Add(Factory.Create<TypeNameItemViewModel>(item.Value.Id, item.Value.Name));
                }
            });
        }

        private async void OpenInfoAction(IViewModel arg) =>
            await this.Navigator.NavigateAsync<ItemInfoViewModel>(arg.As<TypeNameItemViewModel>().ItemId);
    }
}