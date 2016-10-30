using Cauldron.Core;
using Cauldron.XAML.ViewModels;
using System.ComponentModel;
using System.Reflection;

namespace Cauldron.XAML.Validation.ViewModels
{
    /// <summary>
    /// Represents a viewmodel that is able to validate property values
    /// </summary>
    public interface IValidatableViewModel : IViewModel, INotifyDataErrorInfo, IDisposableObject
    {
        /// <summary>
        /// Starts a validation on all properties
        /// </summary>
        void Validate();

        /// <summary>
        /// Starts a validation on all properties
        /// </summary>
        /// <param name="propertyName">The name of the property that requires validation</param>
        void Validate(string propertyName);

        /// <summary>
        /// Starts a validation on all properties
        /// </summary>
        /// <param name="sender">The property info of the property that requested a validation</param>
        /// <param name="propertyName">The name of the property that requires validation</param>
        void Validate(PropertyInfo sender, string propertyName);
    }
}