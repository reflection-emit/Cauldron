using Cauldron.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Cauldron
{
    /// <summary>
    /// Represents a class that handles the creation of a new <see cref="Window"/> and association of the viewmodel
    /// </summary>
    public interface INavigator
    {
        /// <summary>
        /// Closes the current focused <see cref="Window"/>.
        /// </summary>
        void CloseFocusedWindow();

        /// <summary>
        /// Closes the window to where the given viewmodel was directly assigned to.
        /// </summary>
        /// <param name="viewModel">The viewmodel to that was assigned to the window's data context</param>
        /// <returns>Returns true if <see cref="Window.Close"/> was triggered, otherwise false</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="viewModel"/> is null</exception>
        bool CloseWindowOf(IViewModel viewModel);

        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <param name="viewModelType">The viewModel type to create</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        void Navigate(Type viewModelType);

        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        void Navigate<T>() where T : IViewModel;

        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <typeparam name="TResult">The result type of the dialog</typeparam>
        /// <param name="callback">A delegate that is called after the <see cref="Window"/> has been closed</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        void Navigate<T, TResult>(Action<TResult> callback) where T : IViewModel;

        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <param name="args">Parameters of the <see cref="NavigatingAttribute"/></param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        void Navigate<T>(params object[] args) where T : IViewModel;

        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <typeparam name="TResult">The result type of the dialog</typeparam>
        /// <param name="callback">A delegate that is called after the <see cref="Window"/> has been closed</param>
        /// <param name="args">Parameters of the <see cref="NavigatingAttribute"/></param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        /// <exception cref="ResourceReferenceKeyNotFoundException">View of a viewmodel not found</exception>
        void Navigate<T, TResult>(Action<TResult> callback, params object[] args) where T : IViewModel;
    }
}