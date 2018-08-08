using Cauldron.Threading;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cauldron.XAML.ViewModels
{
    /// <summary>
    /// Defines a ViewModel
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged, INotifyBehaviourInvocation
    {
        /// <summary>
        /// Occures if the <see cref="IsLoading"/> property has changed.
        /// </summary>
        event EventHandler IsLoadingChanged;

        /// <summary>
        /// Gets the <see cref="Dispatcher"/> this <see cref="IDispatcher"/> is associated with.
        /// </summary>
        IDispatcher Dispatcher { get; }

        /// <summary>
        /// Gets the unique Id of the view model
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets or sets a value that indicates if the viewmodel is loading
        /// </summary>
        bool IsLoading { get; set; }

        /// <summary>
        /// Centralized error handling
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> that occured</param>
        void OnException(Exception e);

        /// <summary>
        /// Invokes the <see cref="INotifyBehaviourInvocation.BehaviourInvoke"/> event
        /// </summary>
        /// <param name="behaviourName"></param>
        void RaiseNotifyBehaviourInvoke(string behaviourName);

        /// <summary>
        /// Invokes the <see cref="INotifyPropertyChanged.PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        void RaisePropertyChanged([CallerMemberName]string propertyName = "");
    }
}