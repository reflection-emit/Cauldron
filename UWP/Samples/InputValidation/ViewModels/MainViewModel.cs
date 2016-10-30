using Cauldron.Core;
using Cauldron.XAML;
using Cauldron.XAML.Validation;
using Cauldron.XAML.Validation.ViewModels;
using System.Windows.Input;

namespace InputValidation.ViewModels
{
    public class MainViewModel : ValidatableViewModelBase
    {
        public MainViewModel()
        {
            this.IsLoading = false;
            this.ExecuteCommand = new RelayCommand(this.ExecuteAction);
        }

        /// <summary>
        /// Gets the Execute command
        /// </summary>
        public ICommand ExecuteCommand { get; private set; }

        [IsMandatory("mandatory")]
        [StringLength(8, "lengthMustBe8")]
        public string Name { get; set; }

        [GreaterThanOrEqual(10, "greaterThan")]
        [LessThanOrEqual(100, "lessThan")]
        public int Number { get; set; }

        private async void ExecuteAction()
        {
            this.Validate();

            if (this.HasErrors)
                return;

            await this.MessageDialog.ShowAsync("Comments", "perfect", MessageBoxImage.Information, new CauldronUICommand("OK"), null);
        }
    }
}