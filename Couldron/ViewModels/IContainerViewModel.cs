using System;

namespace Couldron.ViewModels
{
    /// <summary>
    /// Represents a viewmodel that can contain child view models
    /// </summary>
    public interface IContainerViewModel : IViewModel
    {
        /// <summary>
        /// Returns a registered Child ViewModel
        /// </summary>
        /// <typeparam name="T">The type of the viewModel</typeparam>
        /// <returns>The viewModel otherwise null</returns>
        T GetRegistered<T>() where T : class, IViewModel;

        /// <summary>
        /// Returns a registered child viewmodel
        /// </summary>
        /// <param name="id">The id of the viewmodel</param>
        /// <returns>The viewmodel otherwise null</returns>
        IViewModel GetRegistered(Guid id);

        /// <summary>
        /// Registers a child model to the current ViewModel
        /// </summary>
        /// <param name="childViewModel">The view model that requires registration</param>
        /// <returns>The id of the viewmodel</returns>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="childViewModel"/> is null</exception>
        Guid Register(IViewModel childViewModel);

        /// <summary>
        /// Unregisters a registered viewmodel. This will also dispose the viewmodel.
        /// </summary>
        /// <param name="childId">The id of the registered viewmodel</param>
        void UnRegister(Guid childId);

        /// <summary>
        /// Unregisters a registered viewmodel. This will also dispose the viewmodel.
        /// </summary>
        /// <param name="childViewModel">The viewmodel that requires unregistration</param>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="childViewModel"/> is null</exception>
        void UnRegister(IViewModel childViewModel);
    }
}