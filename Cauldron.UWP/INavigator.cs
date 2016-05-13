using Cauldron.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Cauldron
{
    /// <summary>
    /// Represents a class that handles the creation of a new <see cref="Page"/> and association of the viewmodel
    /// </summary>
    public interface INavigator
    {
        /// <summary>
        /// Handles creation of a new <see cref="Page"/> and association of the viewmodel
        /// </summary>
        /// <param name="viewModelType">The viewModel type to create</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        void Navigate(Type viewModelType);

        /// <summary>
        /// Handles creation of a new <see cref="Page"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        void Navigate<T>() where T : IViewModel;

        /// <summary>
        /// Handles creation of a new <see cref="Page"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <typeparam name="TResult">The result type of the dialog</typeparam>
        /// <param name="callback">A delegate that is called after the <see cref="Page"/> has been closed</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        void Navigate<T, TResult>(Action<TResult> callback) where T : IViewModel;

        /// <summary>
        /// Handles creation of a new <see cref="Page"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <param name="args">Parameters of the <see cref="NavigatingAttribute"/></param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        void Navigate<T>(params object[] args) where T : IViewModel;

        /// <summary>
        /// Handles creation of a new <see cref="Page"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <typeparam name="TResult">The result type of the dialog</typeparam>
        /// <param name="callback">A delegate that is called after the <see cref="Page"/> has been closed</param>
        /// <param name="args">Parameters of the <see cref="NavigatingAttribute"/></param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        /// <exception cref="ResourceReferenceKeyNotFoundException">View of a viewmodel not found</exception>
        void Navigate<T, TResult>(Action<TResult> callback, params object[] args) where T : IViewModel;
    }
}