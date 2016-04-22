using Cauldron.Core;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cauldron.ViewModels
{
    /// <summary>
    /// Defines a ViewModel
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged, INotifyBehaviourInvokation
    {
        /// <summary>
        /// Gets the <see cref="Dispatcher"/> this <see cref="CauldronDispatcher"/> is associated with.
        /// </summary>
        CauldronDispatcher Dispatcher { get; }

        /// <summary>
        /// Gets the unique Id of the view model
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Invokes the <see cref="INotifyPropertyChanged.PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        void OnPropertyChanged([CallerMemberName]string propertyName = "");
    }
}