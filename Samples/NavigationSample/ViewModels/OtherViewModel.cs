using Couldron;
using Couldron.Validation;
using Couldron.ViewModels;
using System;
using System.Windows.Input;

namespace NavigationSample.ViewModels
{
    [Navigating(nameof(Navigate))]
    public class OtherViewModel : ValidatableViewModelBase, IDialogViewModel<string>, IWindowViewModel
    {
        public OtherViewModel()
        {
            this.BackCommand = new RelayCommand(this.BackAction);
        }

        public ICommand BackCommand { get; private set; }

        [IsMandatory("mandatory")]
        public string Result { get; set; }

        [IsMandatory("mandatory")]
        public DateTime TestTime { get; set; }

        public void Activated()
        {
        }

        public bool CanClose()
        {
            return !this.HasErrors;
        }

        public void Deactivated()
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
            Navigator.CloseFocusedWindow();
        }
    }
}