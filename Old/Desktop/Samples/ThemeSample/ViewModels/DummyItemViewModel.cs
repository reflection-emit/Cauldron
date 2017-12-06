using Cauldron.XAML.ViewModels;

namespace ThemeSample.ViewModels
{
    public sealed class DummyItemViewModel : ViewModelBase
    {
        public DummyItemViewModel(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}