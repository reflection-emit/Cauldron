using System.ComponentModel;
using System.Windows.Input;

namespace Cauldron.XAML
{
    /// <summary>
    /// Defines a command
    /// </summary>
    public interface IRelayCommand : ICommand, INotifyPropertyChanged
    {
        /// <summary>
        /// Triggers the <see cref="ICommand.CanExecuteChanged"/> event and forces the control to refresh the execution state
        /// </summary>
        void RefreshCanExecute();
    }
}