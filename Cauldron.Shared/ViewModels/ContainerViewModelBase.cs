using System;

namespace Cauldron.ViewModels
{
    /// <summary>
    /// Represents the Base class of a ViewModel that can have registered child viewmodels
    /// </summary>
    public abstract class ContainerViewModelBase : DisposableViewModelBase, IContainerViewModel
    {
        private ViewModelContainerHandler handler;

        /// <summary>
        /// Initializes a new instance of <see cref="ContainerViewModelBase"/>
        /// </summary>
        /// <param name="id">A unique identifier of the viewmodel</param>
        public ContainerViewModelBase(Guid id) : base(id)
        {
            this.handler = new ViewModelContainerHandler(this);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ContainerViewModelBase"/>
        /// </summary>
        [Inject]
        public ContainerViewModelBase() : base()
        {
            this.handler = new ViewModelContainerHandler(this);
        }

        /// <summary>
        /// Returns a registered Child ViewModel
        /// </summary>
        /// <typeparam name="T">The type of the viewModel</typeparam>
        /// <returns>The viewModel otherwise null</returns>
        public T GetRegistered<T>() where T : class, IViewModel
        {
            return this.handler.GetRegistered<T>();
        }

        /// <summary>
        /// Returns a registered child viewmodel
        /// </summary>
        /// <param name="id">The id of the viewmodel</param>
        /// <returns>The viewmodel otherwise null</returns>
        public IViewModel GetRegistered(Guid id)
        {
            return this.handler.GetRegistered(id);
        }

        /// <summary>
        /// Registers a child model to the current ViewModel
        /// </summary>
        /// <param name="childViewModel">The view model that requires registration</param>
        /// <returns>The id of the viewmodel</returns>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="childViewModel"/> is null</exception>
        public Guid Register(IViewModel childViewModel)
        {
            return this.handler.Register(childViewModel);
        }

        /// <summary>
        /// Unregisters a registered viewmodel. This will also dispose the viewmodel.
        /// </summary>
        /// <param name="childId">The id of the registered viewmodel</param>
        public void UnRegister(Guid childId)
        {
            this.handler.UnRegister(childId);
        }

        /// <summary>
        /// Unregisters a registered viewmodel. This will also dispose the viewmodel.
        /// </summary>
        /// <param name="childViewModel">The viewmodel that requires unregistration</param>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="childViewModel"/> is null</exception>
        public void UnRegister(IViewModel childViewModel)
        {
            this.UnRegister(childViewModel);
        }

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            if (disposeManaged)
                this.handler.Dispose();
        }
    }
}