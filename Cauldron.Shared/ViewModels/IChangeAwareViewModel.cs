using System;

namespace Cauldron.ViewModels
{
    /// <summary>
    /// Represents a viewmodel that is aware of changes
    /// </summary>
    public interface IChangeAwareViewModel : IViewModel
    {
        /// <summary>
        /// Occures when a value has changed
        /// </summary>
        event EventHandler Changed;

        /// <summary>
        /// Gets or sets a value that indicates if any property value has been changed
        /// </summary>
        bool IsChanged { get; }

        /// <summary>
        /// Gets or sets a value that indicates if the viewmodel is loading
        /// </summary>
        bool IsLoading { get; set; }

        /// <summary>
        /// Gets or sets the parent viewmodel
        /// </summary>
        IViewModel Parent { get; set; }
    }
}