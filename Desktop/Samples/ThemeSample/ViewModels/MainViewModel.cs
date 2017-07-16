using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.XAML;
using Cauldron.XAML.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using ThemeSample.Views;

namespace ThemeSample.ViewModels
{
    [View(typeof(MainView))]
    public sealed class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            this.Tabs.Add(new ListBoxTestViewModel());
            this.Tabs.Add(new ListViewTestViewModel());

            this.SelectedTab = this.Tabs.FirstOrDefault();

            MessageManager.Subscribe<CreateNewTabMessageArgs>(this, x =>
            {
                var vm = Factory.Create(x.ViewModelType) as IViewModel;
                this.Tabs.Remove(x.Sender as IViewModel);
                this.Tabs.Add(vm);

                this.SelectedTab = vm;
            });
        }

        public IViewModel SelectedTab { get; set; }

        public ObservableCollection<IViewModel> Tabs { get; private set; } = new ObservableCollection<IViewModel>();
    }
}