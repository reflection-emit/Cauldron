using Couldron.Core;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Couldron.ViewModels
{
    /// <summary>
    /// Handles the children of viewmodels
    /// </summary>
    public sealed class ViewModelContainerHandler : DisposableBase
    {
        private ConcurrentDictionary<Guid, IViewModel> childViewModels = new ConcurrentDictionary<Guid, IViewModel>();

        private IViewModel context;

        /// <summary>
        /// Initializes a new instance of <see cref="ViewModelContainerHandler"/> class
        /// </summary>
        /// <param name="context">The viewmodel that will contain child viewmodels</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="context"/> is null</exception>
        public ViewModelContainerHandler(IViewModel context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        /// <summary>
        /// Occures when a value has changed
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Gets the parent of the object
        /// </summary>
        public ViewModelBase Parent { get; internal set; }

        /// <summary>
        /// Returns a registered Child ViewModel
        /// </summary>
        /// <typeparam name="T">The type of the viewModel</typeparam>
        /// <returns>The viewModel otherwise null</returns>
        public T GetRegistered<T>() where T : class, IViewModel
        {
            var keyValue = this.childViewModels.FirstOrDefault(x => x is T);

            if (keyValue.Value == null)
                return null;

            return keyValue.Value as T;
        }

        /// <summary>
        /// Returns a registered child viewmodel
        /// </summary>
        /// <param name="id">The id of the viewmodel</param>
        /// <returns>The viewmodel otherwise null</returns>
        public IViewModel GetRegistered(Guid id)
        {
            if (!this.childViewModels.ContainsKey(id))
                return null;

            return this.childViewModels[id];
        }

        /// <summary>
        /// Registers a child model to the current ViewModel
        /// </summary>
        /// <param name="childViewModel">The view model that requires registration</param>
        /// <returns>The id of the viewmodel</returns>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="childViewModel"/> is null</exception>
        public Guid Register(IViewModel childViewModel)
        {
            if (childViewModel == null)
                throw new ArgumentNullException(nameof(childViewModel));

            (childViewModel as IChangeAwareViewModel).IsNotNull(x =>
                {
                    x.Parent = this.context;
                    x.Changed += this.RegisteredInstanceChanged;
                });

            this.childViewModels.TryAdd(childViewModel.Id, childViewModel);

            return childViewModel.Id;
        }

        /// <summary>
        /// Unregisters a registered viewmodel. This will also dispose the viewmodel.
        /// </summary>
        /// <param name="childViewModel">The viewmodel that requires unregistration</param>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="childViewModel"/> is null</exception>
        public void UnRegister(IViewModel childViewModel)
        {
            if (childViewModel == null)
                throw new ArgumentNullException(nameof(childViewModel));

            this.UnRegister(childViewModel.Id);
        }

        /// <summary>
        /// Unregisters a registered viewmodel. This will also dispose the viewmodel.
        /// </summary>
        /// <param name="childId">The id of the registered viewmodel</param>
        public void UnRegister(Guid childId)
        {
            if (this.childViewModels.ContainsKey(childId))
            {
                IViewModel value;
                this.childViewModels.TryRemove(childId, out value);

                (value as IChangeAwareViewModel).IsNotNull(x =>
                {
                    x.Parent = null;
                    x.Changed -= this.RegisteredInstanceChanged;
                    x.DisposeAll();
                });
            }
        }

        /// <summary>
        /// Starts a validation on all properties
        /// </summary>
        public void Validate()
        {
            foreach (object item in this.childViewModels)
                (item as IValidatableViewModel).IsNotNull(x => x.Validate());
        }

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            foreach (var item in this.childViewModels)
                item.DisposeAll();

            this.childViewModels.Clear();
            this.context = null;
        }

        private void RegisteredInstanceChanged(object sender, EventArgs e)
        {
            if (this.Changed != null)
                this.Changed(sender, EventArgs.Empty);
        }
    }
}