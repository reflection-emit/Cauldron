using Couldron.Core;
using Couldron.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Couldron.ViewModels
{
    /// <summary>
    /// Handles validation of a viewmodel
    /// </summary>
    public sealed class ValidationHandler : DisposableBase
    {
        private IValidatableViewModel context;

        /// <summary>
        /// Initializes a new instance of <see cref="ValidationHandler"/> class
        /// </summary>
        /// <param name="context">The viewmodel that contains the properties that requires validation</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="context"/> is null</exception>
        public ValidationHandler(IValidatableViewModel context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.context = context;
            this.AddValidators();
        }

        #region Validation

        private Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
        private object syncValidationRoot = new object();
        private ValidatorCollection validators = new ValidatorCollection();

        /// <summary>
        /// Occures if the count of the errors has changed
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Occures on validation
        /// </summary>
        public event EventHandler<ValidationEventArgs> Validation;

        /// <summary>
        /// Gets or sets the error info strings
        /// </summary>
        public string Errors { get; private set; }

        /// <summary>
        /// Gets a value that indicates if the ViewModel has errors after validation
        /// </summary>
        public bool HasErrors
        {
            get { return errors.SelectMany(x => x.Value).Any(); }
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for; or null or Empty, to retrieve entity-level errors.</param>
        /// <returns>The validation errors for the property or entity.</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName) || !errors.ContainsKey(propertyName))
                return null;

            return this.errors[propertyName];
        }

        /// <summary>
        /// Starts a validation on all properties
        /// </summary>
        /// <param name="propertyName">The name of the property that requires validation</param>
        public void Validate(string propertyName)
        {
            this.Validate(null, propertyName);
        }

        /// <summary>
        /// Starts a validation on all properties
        /// </summary>
        /// <param name="sender">The property info of the property that requested a validation</param>
        /// <param name="propertyName">The name of the property that requires validation</param>
        public void Validate(PropertyInfo sender, string propertyName)
        {
            lock (syncValidationRoot)
            {
                if (this.errors.ContainsKey(propertyName))
                    this.errors.Remove(propertyName);

                this.validators
                    .FirstOrDefault(x => x.PropertyName == propertyName)
                    .IsNotNull(x =>
                    {
                        foreach (var item in x)
                        {
                            string error = item.Validate(sender, this.context);

                            if (!string.IsNullOrEmpty(error))
                            {
                                this.AddError(propertyName, error);
                                break;
                            }
                        }
                    });

                this.RaiseErrorsChanged(propertyName);
            }

            if (this.Validation != null)
                this.Validation(this, new ValidationEventArgs(propertyName));
        }

        /// <summary>
        /// Starts a validation on all properties
        /// </summary>
        public void Validate()
        {
            lock (syncValidationRoot)
            {
                this.errors.Clear();
                this.RaiseErrorsChanged("");

                foreach (var validatorGroup in this.validators)
                {
                    foreach (var validator in validatorGroup)
                    {
                        string error = validator.Validate(null, this.context);

                        if (!string.IsNullOrEmpty(error))
                            this.AddError(validator.propertyInfo.Name, error);
                    }
                }
            }

            if (this.Validation != null)
                this.Validation(this, new ValidationEventArgs());
        }

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                this.context = null;
                this.errors.Clear();
                this.validators.Clear();
            }
        }

        private void AddError(string propertyName, string error)
        {
            if (!this.errors.ContainsKey(propertyName))
                this.errors[propertyName] = new List<string>();

            if (!this.errors[propertyName].Contains(error))
            {
                this.errors[propertyName].Add(error);
                this.RaiseErrorsChanged(propertyName);
            }
        }

        private void AddValidators()
        {
            lock (syncValidationRoot)
            {
                foreach (var property in this.context.GetType().GetProperties())
                {
                    foreach (ValidationBaseAttribute attrib in property.GetCustomAttributes(false).Where(x => (x as ValidationBaseAttribute) != null))
                    {
                        if (attrib == null)
                            continue;

                        this.Validators(property, attrib);
                    }
                }
            }
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            this.Errors = string.Join("\r\n", this.errors.Values.SelectMany(x => x));

            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void Validators(PropertyInfo propertyInfo, ValidationBaseAttribute validationAttribute)
        {
            validationAttribute.propertyInfo = propertyInfo;

            if (!this.validators.Contains(propertyInfo.Name))
                this.validators.Add(new ValidatorGroup(propertyInfo.Name));

            this.validators[propertyInfo.Name].Add(validationAttribute);
        }

        #endregion Validation
    }
}