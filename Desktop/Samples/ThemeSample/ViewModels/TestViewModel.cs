using Cauldron.Core;
using Cauldron.XAML;
using Cauldron.XAML.ViewModels;
using System.Windows.Input;

namespace ThemeSample.ViewModels
{
    public class TestViewModel : ViewModelBase
    {
        public TestViewModel()
        {
            this.CreateNewListBoxTabCommand = new RelayCommand(this.CreateNewListBoxTabAction);
        }

        /// <summary>
        /// Gets the CreateNewListBoxTab command
        /// </summary>
        public ICommand CreateNewListBoxTabCommand { get; private set; }

        public string Title { get { return "Tab Creator"; } }

        private void CreateNewListBoxTabAction() => MessageManager.Send(new CreateNewTabMessageArgs(this, typeof(ListBoxTestViewModel)));
    }
}