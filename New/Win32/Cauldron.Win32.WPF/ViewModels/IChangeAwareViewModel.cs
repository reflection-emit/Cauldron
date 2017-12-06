using System;
using System.ComponentModel;

/* TODO */

namespace Cauldron.XAML.ViewModels
{
    /// <summary>
    /// Represents a viewmodel that is aware of changes.
    /// </summary>
    public interface IChangeAwareViewModel : IViewModel
    {
        /// <summary>
        /// Occures when a value has changed
        /// </summary>
        event EventHandler<PropertyIsChangedEventArgs> Changed;

        /// <summary>
        /// Gets or sets a value that indicates if any property value has been changed
        /// </summary>
        bool IsChanged { get; set; }

        /// <summary>
        /// Invokes the <see cref="INotifyPropertyChanged.PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        /// <param name="before">The value before the property value has changed</param>
        /// <param name="after">The value after the property value has changed</param>
        void RaisePropertyChanged(string propertyName, object before, object after);
    }
}