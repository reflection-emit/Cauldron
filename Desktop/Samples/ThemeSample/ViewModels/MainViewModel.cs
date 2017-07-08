using Cauldron.XAML;
using Cauldron.XAML.ViewModels;
using System.Collections.ObjectModel;
using ThemeSample.Views;

namespace ThemeSample.ViewModels
{
    [View(typeof(MainView))]
    public sealed class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            this.ListViewItems.Add(new TestListViewViewModel { Property1 = "Item 1", Property2 = 33, Property3 = 66.4, Property4 = true });
            this.ListViewItems.Add(new TestListViewViewModel { Property1 = "Item 2", Property2 = 234, Property3 = 99.8834, Property4 = false });
            this.ListViewItems.Add(new TestListViewViewModel { Property1 = "Item 3", Property2 = 88, Property3 = 9234.33, Property4 = false });
            this.ListViewItems.Add(new TestListViewViewModel { Property1 = "Item 4", Property2 = 923, Property3 = 11.334, Property4 = false });
            this.ListViewItems.Add(new TestListViewViewModel { Property1 = "Item 5", Property2 = 91244, Property3 = 03943.6, Property4 = true });

            this.ListBoxItems.Add("Test 1");
            this.ListBoxItems.Add("Test 2");
            this.ListBoxItems.Add("Test 3");
            this.ListBoxItems.Add("Test 4");
        }

        public ObservableCollection<string> ListBoxItems { get; private set; } = new ObservableCollection<string>();
        public ObservableCollection<TestListViewViewModel> ListViewItems { get; private set; } = new ObservableCollection<TestListViewViewModel>();
    }
}