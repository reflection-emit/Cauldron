using Cauldron.Core;
using Cauldron.Core.Diagnostics;
using Cauldron.Interception;
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Cauldron.XAML.ViewModels
{
    /// <summary>
    /// Registers the property to the declaring viewmodel.
    /// A registered child will able to propagate changes of its <see cref="IViewModel.IsLoading"/> and <see cref="IChangeAwareViewModel.IsChanged"/> property to its parent viewmodel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class RegisterChildrenAttribute : Attribute, IPropertySetterInterceptor, IPropertyInterceptorInitialize
    {
        private IViewModel _context;
        private object children;
        private bool propagatesIsChange;
        private bool propagatesIsLoading;

        private string propertyName;

        /// <summary>
        /// Initializes a new instance of <see cref="RegisterChildrenAttribute"/>.
        /// </summary>
        /// <param name="propagatesIsChange">If true, changes to the <see cref="IChangeAwareViewModel.IsChanged"/> property will be propagated to the parent viewmodel.</param>
        /// <param name="propagatesIsLoading">If true, changes to the <see cref="IViewModel.IsLoading"/> property will be propagated to the parent viewmodel.</param>
        /// <exception cref="Exception">Attributed property is static.</exception>
        public RegisterChildrenAttribute(bool propagatesIsChange = false, bool propagatesIsLoading = false)
        {
            this.propagatesIsChange = propagatesIsChange;
            this.propagatesIsLoading = propagatesIsLoading;
        }

        /// <summary>
        /// Gets the declaring viewmodel instance.
        /// </summary>
        public IViewModel Context
        {
            get { return this._context; }
            private set
            {
                if (this._context == value)
                    return;

                // Deregister the events first
                if (this._context != null && this._context is IChangeAwareViewModel changeAwareBefore)
                    changeAwareBefore.IsChangedChanged -= ParentIsChangeChanged;

                if (this._context != null && this._context is IDisposableObject disposableBefore)
                    disposableBefore.Disposed -= Parent_Disposed;

                this._context = value;

                if (this._context != null && this._context is IChangeAwareViewModel changeAwareAfter)
                    changeAwareAfter.IsChangedChanged += ParentIsChangeChanged;

                if (this._context != null && this._context is IDisposableObject disposableAfter)
                    disposableAfter.Disposed += Parent_Disposed;
            }
        }

        /// <exclude/>
        public void OnException(Exception e)
        {
        }

        /// <exclude/>
        public void OnExit()
        {
        }

        /// <summary>
        /// Invoked if the declaring class is initialized.
        /// </summary>
        /// <param name="propertyInterceptionInfo">
        /// An object that containes information about the intercepted method
        /// </param>
        /// <param name="value">The current value of the property</param>
        public void OnInitialize(PropertyInterceptionInfo propertyInterceptionInfo, object value) => this.OnSet(propertyInterceptionInfo, null, value);

        /// <summary>
        /// Invoked if the intercepted property setter has been called
        /// </summary>
        /// <param name="propertyInterceptionInfo">
        /// An object that containes information about the intercepted method
        /// </param>
        /// <param name="oldValue">The current value of the property</param>
        /// <param name="newValue">The to be new value of the property</param>
        /// <returns>If returns false, the backing field will be set to <paramref name="newValue"/></returns>
        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            this.propertyName = propertyInterceptionInfo.PropertyName;
            this.Context = propertyInterceptionInfo.Instance as IViewModel;
            this.children = newValue;

            if (propertyInterceptionInfo.SetMethod.IsStatic)
                throw new Exception("RegisterChildrenAttribute does not support static properties.");

            if (newValue == null)
                return false;

            if (oldValue != null)
                this.DetachEvents(oldValue);

            if (newValue is INotifyCollectionChanged specializedCollection)
                specializedCollection.CollectionChanged += SpecializedCollection_CollectionChanged;
            else if (newValue is IEnumerable)
                Debug.WriteLine($"RegisterChildrenAttribute: The type of the attributed property '{propertyInterceptionInfo.PropertyName}' does not implement the 'INotifyCollectionChanged' interface. The elements will be registered, but changes cannot be monitored.");

            foreach (var item in newValue is IEnumerable enumerable ? enumerable : new object[] { newValue })
            {
                if (this.propagatesIsLoading && item is IViewModel vm)
                    vm.IsLoadingChanged -= ChildIsLoadingChanged;

                if (this.propagatesIsChange && item is IChangeAwareViewModel changeAware)
                    changeAware.IsChangedChanged -= ChildIsChangedChanged;
            }

            return false;
        }

        private void ChildIsChangedChanged(object sender, EventArgs e)
        {
            if (this.propagatesIsChange && this.Context is IChangeAwareViewModel changeAware)
                changeAware.IsChanged |= (sender as IChangeAwareViewModel)?.IsChanged ?? false;
        }

        private void ChildIsLoadingChanged(object sender, EventArgs e)
        {
            if (this.propagatesIsLoading && this.Context != null)
                this.Context.IsLoading = (sender as IViewModel)?.IsLoading ?? false;
        }

        private void DetachEvents(object value)
        {
            if (value is INotifyCollectionChanged notiyCollection)
                notiyCollection.CollectionChanged -= SpecializedCollection_CollectionChanged;

            foreach (var item in value is IEnumerable enumerable ? enumerable : new object[] { value })
            {
                if (this.propagatesIsLoading && item is IViewModel vm)
                    vm.IsLoadingChanged -= ChildIsLoadingChanged;

                if (this.propagatesIsChange && item is IChangeAwareViewModel changeAware)
                    changeAware.IsChangedChanged -= ChildIsChangedChanged;
            }
        }

        private void Parent_Disposed(object sender, EventArgs e)
        {
            (sender as IDisposableObject).Disposed -= Parent_Disposed;
            this.Context = null;
            this.DetachEvents(this.children);
        }

        private void ParentIsChangeChanged(object sender, EventArgs e)
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

        private void SpecializedCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                foreach (var item in e.NewItems)
                {
                    if (this.propagatesIsLoading && item is IViewModel vm)
                        vm.IsLoadingChanged += ChildIsLoadingChanged;

                    if (this.propagatesIsChange && item is IChangeAwareViewModel changeAware)
                        changeAware.IsChangedChanged += ChildIsChangedChanged;
                }

            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
                foreach (var item in e.OldItems)
                {
                    if (this.propagatesIsLoading && item is IViewModel vm)
                        vm.IsLoadingChanged -= ChildIsLoadingChanged;

                    if (this.propagatesIsChange && item is IChangeAwareViewModel changeAware)
                        changeAware.IsChangedChanged -= ChildIsChangedChanged;
                }

            if (!string.IsNullOrEmpty(this.propertyName))
                this._context?.RaisePropertyChanged(this.propertyName);
        }
    }
}