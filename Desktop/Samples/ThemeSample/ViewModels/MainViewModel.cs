using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.XAML;
using Cauldron.XAML.Validation;
using Cauldron.XAML.Validation.ViewModels;
using Cauldron.XAML.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Windows.Input;
using ThemeSample.Views;

namespace ThemeSample.ViewModels
{
    [View(typeof(MainView))]
    public sealed class MainViewModel : ValidatableViewModelBase
    {
        public MainViewModel()
        {
            this.ValidateCommand = new RelayCommand(this.ValidateAction);

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

            this.Items.Add(new DummyItemViewModel("Elephant"));
            this.Items.Add(new DummyItemViewModel("Tiger"));
            this.Items.Add(new DummyItemViewModel("People"));
            this.Items.Add(new DummyItemViewModel("Trash"));
            this.Items.Add(new DummyItemViewModel("Anything"));
            this.Items.Add(new DummyItemViewModel("Visual Studio"));
            this.Items.Add(new DummyItemViewModel("Cauldron"));
            this.Items.Add(new DummyItemViewModel("Rocks"));
            this.Items.Add(new DummyItemViewModel("The chair"));
            this.Items.Add(new DummyItemViewModel("Cat"));
            this.Items.Add(new DummyItemViewModel("Typhoon"));
            this.Items.Add(new DummyItemViewModel("Minmatar"));
            this.Items.Add(new DummyItemViewModel("Caldari"));
            this.Items.Add(new DummyItemViewModel("Excel in space"));
        }

        [IsMandatory("isMandatory")]
        public IViewModel ComboBoxSelection { get; set; }

        public ObservableCollection<IViewModel> Items { get; private set; } = new ObservableCollection<IViewModel>();

        [Equality(nameof(PasswordB), "mussbeEqual")]
        [IsMandatory("isMandatory")]
        public SecureString PasswordA { get; set; }

        [Equality(nameof(PasswordA), "mussbeEqual")]
        public SecureString PasswordB { get; set; }

        public IViewModel SelectedTab { get; set; }

        public ObservableCollection<IViewModel> Tabs { get; private set; } = new ObservableCollection<IViewModel>();

        public bool ValidateAllToggle
        {
            get { return !ValidationHandler.ValidateAll; }
            set { ValidationHandler.ValidateAll = !value; }
        }

        /// <summary>
        /// Gets the Validate command
        /// </summary>
        public ICommand ValidateCommand { get; private set; }

        [StringLength(6, 10, "error1")]
        public string ValidatedText { get; set; }

        private async void ValidateAction() => await this.ValidateAsync();
    }
}