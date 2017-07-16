using Cauldron.Core;
using Cauldron.XAML.ViewModels;
using System.Collections.ObjectModel;

namespace ThemeSample.ViewModels
{
    public class ListBoxTestViewModel : ViewModelBase, ICloseAwareViewModel
    {
        public ListBoxTestViewModel()
        {
            this.Items.Add("Test 1");
            this.Items.Add("Test 2");
            this.Items.Add("Test 3");
            this.Items.Add("Test 4");
        }

        public ObservableCollection<string> Items { get; private set; } = new ObservableCollection<string>();
        public string Title { get { return "Listbox Test"; } }

        public void Close() => MessageManager.Send(new CreateNewTabMessageArgs(this, typeof(TestViewModel)));
    }
}