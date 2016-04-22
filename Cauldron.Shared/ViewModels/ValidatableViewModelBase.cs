using Cauldron.Validation;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Cauldron.ViewModels
{
    /// <summary>
    /// Represents a base class with an implemented validation
    /// </summary>
    public abstract class ValidatableViewModelBase : DisposableViewModelBase, IValidatableViewModel
    {
        private ValidationHandler validationHandler;

        /// <summary>
        /// Initializes a new instance of <see cref="ValidatableViewModelBase"/>
        /// </summary>
        [Inject]
        public ValidatableViewModelBase() : base()
        {
            this.validationHandler = new ValidationHandler(this);
            this.validationHandler.ErrorsChanged += ValidationHandler_ErrorsChanged;
            this.validationHandler.Validation += ValidationHandler_Validation;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ValidatableViewModelBase"/>
        /// </summary>
        /// <param name="id">A unique identifier of the viewmodel</param>
        public ValidatableViewModelBase(Guid id) : base(id)
        {
            this.validationHandler = new ValidationHandler(this);
            this.validationHandler.ErrorsChanged += ValidationHandler_ErrorsChanged;
            this.validationHandler.Validation += ValidationHandler_Validation;
        }

        /// <summary>
        /// Occures if the count of the errors has changed
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets or sets the error info strings
        /// </summary>
        [SuppressIsChanged, JsonIgnore]
        public string Errors { get { return this.validationHandler.Errors; } }

        /// <summary>
        /// Gets a value that indicates if the ViewModel has errors after validation
        /// </summary>
        [SuppressIsChanged, JsonIgnore]
        public bool HasErrors { get { return this.validationHandler.HasErrors; } }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for; or null or Empty, to retrieve entity-level errors.</param>
        /// <returns>The validation errors for the property or entity.</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            return this.validationHandler.GetErrors(propertyName);
        }

        /// <summary>
        /// Starts a validation on all properties
        /// </summary>
        public void Validate()
        {
            this.validationHandler.Validate();
        }

        /// <summary>
        /// Starts a validation on all properties
        /// </summary>
        /// <param name="propertyName">The name of the property that requires validation</param>
        public void Validate(string propertyName)
        {
            this.validationHandler.Validate(propertyName);
        }

        /// <summary>
        /// Starts a validation on all properties
        /// </summary>
        /// <param name="sender">The property info of the property that requested a validation</param>
        /// <param name="propertyName">The name of the property that requires validation</param>
        public void Validate(PropertyInfo sender, string propertyName)
        {
            this.validationHandler.Validate(sender, propertyName);
        }

        /// <summary>
        /// Occured before the <see cref="ViewModelBase.PropertyChanged"/> event is invoked.
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        /// <returns>Returns true if <see cref="ViewModelBase.OnPropertyChanged(string)"/> should be cancelled. Otherwise false</returns>
        protected override bool OnBeforeRaiseNotifyPropertyChanged(string propertyName)
        {
            // If the application is using weavers like Fody these Properties could be
            // Notified multiple times. We want to trigger the event raises somewhere else
            if (propertyName == nameof(Errors) || propertyName == nameof(HasErrors))
                return false;

            this.validationHandler.Validate(propertyName);

            return base.OnBeforeRaiseNotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                this.validationHandler.ErrorsChanged -= ValidationHandler_ErrorsChanged;
                this.validationHandler.Validation -= ValidationHandler_Validation;
                this.validationHandler.Dispose();
            }
        }

        /// <summary>
        /// Occures on validation
        /// </summary>
        /// <param name="propertyName">The property name that is going to be validated</param>
        protected virtual void OnValidation(string propertyName)
        {
        }

        private void ValidationHandler_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            this.OnPropertyChanged(nameof(Errors));
            this.OnPropertyChanged(nameof(HasErrors));

            if (this.ErrorsChanged != null)
                this.ErrorsChanged(this, e);
        }

        private void ValidationHandler_Validation(object sender, ValidationEventArgs e)
        {
            this.OnValidation(e.PropertyName);
        }
    }
}