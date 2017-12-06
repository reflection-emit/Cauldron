using Cauldron.Core.Diagnostics;
using Cauldron.Interception;
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Cauldron.XAML.ViewModels
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class RegisterChildrenAttribute : Attribute, IPropertySetterInterceptor, IPropertyInterceptorInitialize
    {
        private IViewModel _context;
        private object children;
        private bool propagatesIsChange;
        private bool propagatesIsLoading;

        /// <summary>
        ///
        /// </summary>
        /// <param name="propagatesIsChange"></param>
        /// <param name="propagatesIsLoading"></param>
        /// <exception cref="Exception">Attributed property is static.</exception>
        public RegisterChildrenAttribute(bool propagatesIsChange = false, bool propagatesIsLoading = false)
        {
            this.propagatesIsChange = propagatesIsChange;
            this.propagatesIsLoading = propagatesIsLoading;
        }

        public IViewModel Context
        {
            get { return this._context; }
            private set
            {
                if (this._context == value)
                    return;

                // Deregister the events first
                if (this._context != null && this._context is IChangeAwareViewModel changeAwareBefore)
                    changeAwareBefore.Changed -= ChangeAware_Changed;

                this._context = value;

                if (this._context != null && this._context is IChangeAwareViewModel changeAwareAfter)
                    changeAwareAfter.Changed += ChangeAware_Changed;
            }
        }

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }

        public void OnInitialize(PropertyInterceptionInfo propertyInterceptionInfo, object value) => this.OnSet(propertyInterceptionInfo, null, value);

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            this.Context = propertyInterceptionInfo.Instance as IViewModel;
            this.children = newValue;

            if (propertyInterceptionInfo.SetMethod.IsStatic)
                throw new Exception("RegisterChildrenAttribute does not support static properties.");

            if (newValue == null)
                return false;

            if (newValue is IEnumerable enumerable)
            {
                if (newValue is INotifyCollectionChanged specializedCollection)
                    specializedCollection.CollectionChanged += SpecializedCollection_CollectionChanged;
                else
                {
                    Debug.WriteLine($"RegisterChildrenAttribute: The type of the attributed property '{propertyInterceptionInfo.PropertyName}' does not implement the 'INotifyCollectionChanged' interface. The elements will be registered, but changes cannot be monitored.");

                    foreach (var item in enumerable)
                    {
                        if (this.propagatesIsLoading && item is IViewModel vm)
                            vm.IsLoadingChanged += Vm_IsLoadingChanged;

                        if (this.propagatesIsChange && item is IChangeAwareViewModel changeAware)
                            changeAware.IsLoadingChanged += ChangeAware_IsLoadingChanged;
                    }
                }
            }
            else
            {
                if (this.propagatesIsLoading && newValue is IViewModel vm)
                    vm.IsLoadingChanged += Vm_IsLoadingChanged;

                if (this.propagatesIsChange && newValue is IChangeAwareViewModel changeAware)
                    changeAware.IsLoadingChanged += ChangeAware_IsLoadingChanged;
            }

            return false;
        }

        private void ChangeAware_Changed(object sender, PropertyIsChangedEventArgs e)
        {
            var changeAwareContext = this._context as IChangeAwareViewModel;

            // We only propagate the IsChange property if it changes to false
            if (changeAwareContext == null || !changeAwareContext.IsChanged)
                return;

            if (this.propagatesIsChange && this.children is IEnumerable enumerable)
                foreach (var item in enumerable)
                {
                    if (item is IChangeAwareViewModel changeAware)
                        changeAware.IsChanged = false;
                }
            else if (this.children is IChangeAwareViewModel changeAware)
                changeAware.IsChanged = false;
        }

        private void ChangeAware_IsLoadingChanged(object sender, EventArgs e)
        {
            if (this.propagatesIsLoading && this.Context != null)
                this.Context.IsLoading = (sender as IViewModel)?.IsLoading ?? false;
        }

        private void SpecializedCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                foreach (var item in e.NewItems)
                {
                    if (this.propagatesIsLoading && item is IViewModel vm)
                        vm.IsLoadingChanged += Vm_IsLoadingChanged;

                    if (this.propagatesIsChange && item is IChangeAwareViewModel changeAware)
                        changeAware.IsLoadingChanged += ChangeAware_IsLoadingChanged;
                }

            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
                foreach (var item in e.OldItems)
                {
                    if (this.propagatesIsLoading && item is IViewModel vm)
                        vm.IsLoadingChanged -= Vm_IsLoadingChanged;

                    if (this.propagatesIsChange && item is IChangeAwareViewModel changeAware)
                        changeAware.IsLoadingChanged -= ChangeAware_IsLoadingChanged;
                }
        }

        private void Vm_IsLoadingChanged(object sender, EventArgs e)
        {
            if (this.propagatesIsChange && this.Context is IChangeAwareViewModel changeAware)
                changeAware.IsChanged |= (sender as IChangeAwareViewModel)?.IsChanged ?? false;
        }
    }
}