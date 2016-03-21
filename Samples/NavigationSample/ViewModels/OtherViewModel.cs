using Couldron;
using Couldron.Aspects;
using Couldron.ViewModels;
using System.Windows.Input;

namespace NavigationSample.ViewModels
{
    [Navigating(nameof(Navigate))]
    public class OtherViewModel : DisposableViewModelBase, IDialogViewModel<string>, IWindowViewModel
    {
        public OtherViewModel()
        {
            this.BackCommand = new RelayCommand(this.BackAction);
        }

        public ICommand BackCommand { get; private set; }

        [NotifyPropertyChanged]
        public string Result { get; set; }

        public bool CanClose()
        {
            return !string.IsNullOrEmpty(this.Result);
        }

        public void GotFocus()
        {
        }

        public void LostFocus()
        {
        }

        public void Navigate(string defaultText)
        {
            this.Result = defaultText;
        }

        public void SizeChanged(double width, double height)
        {
        }

        protected override void OnDispose(bool disposeManaged)
        {
            base.OnDispose(disposeManaged);
        }

        private void BackAction()
        {
            Navigator.Close();
        }
    }
}