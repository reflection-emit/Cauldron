using Couldron.Validation;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Couldron.ViewModels
{
    /// <summary>
    /// Represents a change aware base class
    /// </summary>
    public abstract class ChangeAwareViewModelBase : ViewModelBase, IChangeAwareViewModel
    {
        private bool _isChanged;
        private bool _isLoading;

        /// <summary>
        /// Initializes a new instance of <see cref="ChangeAwareViewModelBase"/>
        /// </summary>
        /// <param name="id">A unique identifier of the viewmodel</param>
        public ChangeAwareViewModelBase(Guid id) : base(id)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ChangeAwareViewModelBase"/>
        /// </summary>
        [InjectionConstructor]
        public ChangeAwareViewModelBase() : base()
        {
        }

        /// <summary>
        /// Occures when a value has changed
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Gets or sets a value that indicates if any property value has been changed
        /// </summary>
        [SuppressIsChanged, JsonIgnore]
        public bool IsChanged
        {
            get { return this._isChanged; }
            protected set
            {
                if (this._isChanged == value || this._isLoading)
                    return;

                this._isChanged = value;
                this.OnIsChangedChanged();
                this.RaiseNotifyPropertyChanged();

                if (this.Changed != null)
                    this.Changed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the viewmodel is loading
        /// </summary>
        [SuppressIsChanged, JsonIgnore]
        public bool IsLoading
        {
            get { return this._isLoading; }
            set
            {
                if (this._isLoading == value)
                    return;

                this._isLoading = value;
                this.RaiseNotifyPropertyChanged();

                this.OnIsLoadingChanged();
            }
        }

        /// <summary>
        /// Gets or sets the parent viewmodel
        /// </summary>
        [JsonIgnore]
        public IViewModel Parent { get; set; }

        /// <summary>
        /// Occures after the event <see cref="INotifyPropertyChanged.PropertyChanged"/> has been invoked
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        protected override void OnAfterRaiseNotifyPropertyChanged(string propertyName)
        {
            var property = this.GetType().GetProperty(propertyName);
            if (property == null || property.GetCustomAttribute<SuppressIsChangedAttribute>() == null)
                this.IsChanged = true;
        }

        /// <summary>
        /// Occures if the Change Property has changed its value
        /// </summary>
        protected virtual void OnIsChangedChanged()
        {
        }

        /// <summary>
        /// Occures if the Loading Property has changed its value
        /// </summary>
        protected virtual void OnIsLoadingChanged()
        {
        }
    }
}