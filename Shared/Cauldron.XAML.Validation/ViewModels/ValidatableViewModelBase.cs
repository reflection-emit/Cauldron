using Cauldron.Core.Extensions;
using Cauldron.XAML.ViewModels;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Cauldron.XAML.Validation.ViewModels
{
    /// <summary>
    /// Represents a base class with an implemented validation
    /// </summary>
    public abstract class ValidatableViewModelBase : ViewModelBase, IValidatableViewModel
    {
        private readonly ValidatorCollection validators = new ValidatorCollection();

        private string _errors;

        /// <summary>
        /// Initializes a new instance of <see cref="ValidatableViewModelBase"/>
        /// </summary>
        public ValidatableViewModelBase() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ValidatableViewModelBase"/>
        /// </summary>
        /// <param name="id">A unique identifier of the viewmodel</param>
        public ValidatableViewModelBase(Guid id) : base(id)
        {
        }

        /// <summary>
        /// Occures if the count of the errors has changed
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets or sets the error info strings
        /// </summary>
        public string Errors
        {
            get { return this._errors; }
            set
            {
                this._errors = value;

                this.RaisePropertyChanged(nameof(Errors));
                this.RaisePropertyChanged(nameof(HasErrors));
            }
        }

        /// <summary>
        /// Gets a value that indicates if the ViewModel has errors after validation
        /// </summary>
        public bool HasErrors { get { return this.validators.Any(x => x.Error.Count > 0); } }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to retrieve validation errors for; or null or Empty, to retrieve
        /// entity-level errors.
        /// </param>
        /// <returns>The validation errors for the property or entity.</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !this.validators.Contains(propertyName) || this.validators[propertyName].Error.Count == 0)
                return null;

            return this.validators[propertyName].Error;
        }

        /// <summary>
        /// Starts a validation on all properties
        /// </summary>
        public async Task ValidateAsync()
        {
            for (int i = 0; i < this.validators.Count; i++)
            {
                var validator = this.validators[i];
                await validator.ValidateAsync(this, null, true);
                this.RaiseErrorsChanged(validator.PropertyName);
            }

            this.RaiseErrorsChanged("");
        }

        /// <summary>
        /// Starts a validation on a property defined by name.
        /// </summary>
        /// <param name="sender">The property info of the property that requested a validation</param>
        /// <param name="propertyName">The name of the property that requires validation</param>
        public async Task ValidateAsync(PropertyInfo sender, string propertyName)
        {
            await this.validators[propertyName].ValidateAsync(this, sender, false);
            this.RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// Starts a validation on a property defined by name.
        /// </summary>
        /// <param name="propertyName">The name of the property that requires validation</param>
        public Task ValidateAsync(string propertyName) => this.ValidateAsync(null, propertyName);

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void AddValidator(string propertyName, ValidatorAttributeBase attribute) => this.validators[propertyName].Add(attribute);

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void AddValidatorGroup(string propertyName)
        {
            if (!this.validators.Contains(propertyName))
                this.validators.Add(new ValidatorGroup(this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)));
        }

        /// <summary>
        /// Occured before the <see cref="ViewModelBase.PropertyChanged"/> event is invoked.
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        /// <returns>
        /// Returns true if <see cref="ViewModelBase.RaisePropertyChanged(string)"/> should be
        /// cancelled. Otherwise false
        /// </returns>
        protected override bool BeforeRaiseNotifyPropertyChanged(string propertyName)
        {
            // If the application is using weavers like Fody these Properties could be Notified
            // multiple times. We want to trigger the event raises somewhere else
            if (propertyName == nameof(Errors) || propertyName == nameof(HasErrors))
                return false;

            this.ValidateAsync(propertyName);
            return base.BeforeRaiseNotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            if (disposeManaged)
                this.validators.Clear();
        }

        /// <summary>
        /// Occures on validation
        /// </summary>
        /// <param name="propertyName">
        /// The property name that is going to be validated. If the <paramref name="propertyName"/>
        /// is empty the validation has occured for all properties.
        /// </param>
        protected virtual void OnValidation(string propertyName)
        {
        }

        private void RaiseErrorsChanged(string propertyName)
        {
#pragma warning disable 4014
            this.Errors = this.validators.SelectMany(x => x.Error).Join("\r\n");
            this.Dispatcher.RunAsync(() => this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName)));
            this.OnValidation(propertyName);
#pragma warning restore 4014
        }
    }
}