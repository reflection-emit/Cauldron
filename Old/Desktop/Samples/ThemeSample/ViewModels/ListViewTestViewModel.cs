using Cauldron.XAML.ViewModels;
using System.Collections.ObjectModel;

namespace ThemeSample.ViewModels
{
    public class ListViewTestViewModel : ViewModelBase
    {
        public ListViewTestViewModel()
        {
            this.Items.Add(new TestListViewViewModel { Property1 = "Item 1", Property2 = 33, Property3 = 66.4, Property4 = true });
            this.Items.Add(new TestListViewViewModel { Property1 = "Item 2", Property2 = 234, Property3 = 99.8834, Property4 = false });
            this.Items.Add(new TestListViewViewModel { Property1 = "Item 3", Property2 = 88, Property3 = 9234.33, Property4 = false });
            this.Items.Add(new TestListViewViewModel { Property1 = "Item 4", Property2 = 923, Property3 = 11.334, Property4 = false });
            this.Items.Add(new TestListViewViewModel { Property1 = "Item 5", Property2 = 91244, Property3 = 03943.6, Property4 = true });
        }

        public ObservableCollection<TestListViewViewModel> Items { get; private set; } = new ObservableCollection<TestListViewViewModel>();
        public string Title { get { return "Listview Test"; } }
    }
}