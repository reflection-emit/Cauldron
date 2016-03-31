using Couldron.ViewModels;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Couldron
{
    /// <summary>
    /// Handles creation of a new <see cref="Page"/> and association of the viewmodel
    /// </summary>
    public static class Navigator
    {
        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        public static async void Navigate<T>() where T : IViewModel
        {
            await NavigateInternal<T, bool>(null, null);
        }

        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <typeparam name="TResult">The result type of the dialog</typeparam>
        /// <param name="callback">A delegate that is called after the <see cref="Window"/> has been closed</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        public static async void Navigate<T, TResult>(Action<TResult> callback) where T : IViewModel
        {
            await NavigateInternal<T, TResult>(callback);
        }

        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <param name="args">Parameters of the <see cref="NavigatingAttribute"/></param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        public static async void Navigate<T>(params object[] args) where T : IViewModel
        {
            await NavigateInternal<T, bool>(null, args);
        }

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
        public static async void Navigate<T, TResult>(Action<TResult> callback, params object[] args) where T : IViewModel
        {
            await NavigateInternal<T, TResult>(callback, args);
        }

        public static async void NavigateByType(Type type)
        {
            // TODO
        }

        public static async void NavigateByType(Type type, params object[] args)
        {
            // TODO
        }

        private static bool IsParameterMatch(object[] args, ParameterInfo[] types)
        {
            if (args.Length != types.Length)
                return false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].GetType() != types[i].ParameterType)
                    return false;
            }

            return true;
        }

        private static async Task NavigateInternal<T, TResult>(Action<TResult> callback, params object[] args) where T : IViewModel
        {
            // TODO
        }
    }
}