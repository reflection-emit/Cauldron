using Couldron.Core;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Couldron.ViewModels
{
    /// <summary>
    /// Defines a ViewModel
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the <see cref="Dispatcher"/> this <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        CouldronDispatcher Dispatcher { get; }

        /// <summary>
        /// Gets the unique Id of the view model
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Invokes the <see cref="INotifyPropertyChanged.PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        void RaiseNotifyPropertyChanged([CallerMemberName]string propertyName = "");
    }
}