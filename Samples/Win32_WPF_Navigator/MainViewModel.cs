using Cauldron.XAML.ViewModels;

namespace Win32_WPF_Navigator
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(string title, int aNumber)
        {
            this.Title = title;
        }

        public string Title { get; private set; }
    }
}