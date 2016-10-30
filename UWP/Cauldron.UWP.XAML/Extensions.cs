using System;
using System.Threading.Tasks;
using System.IO;
using Cauldron.Core.Extensions;
using Cauldron.XAML.ViewModels;
using Cauldron.XAML.Navigation;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;

#if WINDOWS_UWP

using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Data;
using Windows.Storage;

#else

using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

#endif

namespace Cauldron.XAML
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Draws the visuals of a <see cref="FrameworkElement"/> to a <see cref="BitmapImage"/>
        /// </summary>
        /// <param name="element">The <see cref="FrameworkElement"/> whose visuals are drawn to an image</param>
        /// <returns>An image representation of the <see cref="FrameworkElement"/></returns>
        public static async Task<BitmapImage> AsBitmapImage(this FrameworkElement element)
        {
            var bitmapImage = new BitmapImage();
#if WINDOWS_UWP
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(element, (int)element.Width, (int)element.Height);
            var pixels = await renderTargetBitmap.GetPixelsAsync();

            using (var stream = new MemoryStream().AsRandomAccessStream())
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                byte[] bytes = pixels.ToArray();
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                     BitmapAlphaMode.Ignore,
                                     (uint)element.Width, (uint)element.Height,
                                     96, 96, bytes);
                stream.Seek(0);

                await bitmapImage.SetSourceAsync(stream);
                await encoder.FlushAsync();
            }

#else
            await Task.FromResult(0); // HACK

            var source = PresentationSource.FromVisual(element);
            var sourceBrush = new VisualBrush(element);
            var renderTargetBitmap = new RenderTargetBitmap((int)element.RenderSize.Width, (int)element.RenderSize.Height, 96, 96, PixelFormats.Pbgra32);
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(element.RenderSize.Width, element.RenderSize.Height)));
            }

            renderTargetBitmap.Render(drawingVisual);

            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using (var stream = new MemoryStream())
            {
                bitmapEncoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

#endif
            return bitmapImage;
        }

        /// <summary>
        /// Copies a <see cref="BindingBase"/>
        /// </summary>
        /// <param name="bindingBase">The binding to clone</param>
        /// <returns>A new instance of <typeparam name="T"></typeparam></returns>
        public static T Clone<T>(this T bindingBase) where T : BindingBase
        {
            var binding = bindingBase as Binding;
            if (binding != null)
            {
                var result = new Binding
                {
                    Source = null,
                    Converter = binding.Converter,
                    ConverterParameter = binding.ConverterParameter,
                    FallbackValue = binding.FallbackValue,
                    Mode = binding.Mode,
                    Path = binding.Path,
                    TargetNullValue = binding.TargetNullValue,
                    UpdateSourceTrigger = binding.UpdateSourceTrigger,
#if WINDOWS_UWP
                    ConverterLanguage = binding.ConverterLanguage
#else
                    AsyncState = binding.AsyncState,
                    BindingGroupName = binding.BindingGroupName,
                    BindsDirectlyToSource = binding.BindsDirectlyToSource,
                    ConverterCulture = binding.ConverterCulture,
                    IsAsync = binding.IsAsync,
                    NotifyOnSourceUpdated = binding.NotifyOnSourceUpdated,
                    NotifyOnTargetUpdated = binding.NotifyOnTargetUpdated,
                    NotifyOnValidationError = binding.NotifyOnValidationError,
                    StringFormat = binding.StringFormat,
                    UpdateSourceExceptionFilter = binding.UpdateSourceExceptionFilter,
                    ValidatesOnDataErrors = binding.ValidatesOnDataErrors,
                    ValidatesOnExceptions = binding.ValidatesOnExceptions,
                    XPath = binding.XPath,
#endif
                };

#if !WINDOWS_UWP
                foreach (var validationRule in binding.ValidationRules)
                    result.ValidationRules.Add(validationRule);
#endif

                return result as T;
            }

#if !WINDOWS_UWP // No MultiBinding and PriorityBinding in UWP
            var multiBinding = bindingBase as MultiBinding;
            if (multiBinding != null)
            {
                var result = new MultiBinding
                {
                    BindingGroupName = multiBinding.BindingGroupName,
                    Converter = multiBinding.Converter,
                    ConverterCulture = multiBinding.ConverterCulture,
                    ConverterParameter = multiBinding.ConverterParameter,
                    FallbackValue = multiBinding.FallbackValue,
                    Mode = multiBinding.Mode,
                    NotifyOnSourceUpdated = multiBinding.NotifyOnSourceUpdated,
                    NotifyOnTargetUpdated = multiBinding.NotifyOnTargetUpdated,
                    NotifyOnValidationError = multiBinding.NotifyOnValidationError,
                    StringFormat = multiBinding.StringFormat,
                    TargetNullValue = multiBinding.TargetNullValue,
                    UpdateSourceExceptionFilter = multiBinding.UpdateSourceExceptionFilter,
                    UpdateSourceTrigger = multiBinding.UpdateSourceTrigger,
                    ValidatesOnDataErrors = multiBinding.ValidatesOnDataErrors,
                    ValidatesOnExceptions = multiBinding.ValidatesOnDataErrors,
                };

                foreach (var validationRule in multiBinding.ValidationRules)
                    result.ValidationRules.Add(validationRule);

                return result as T;
            }

            var priorityBinding = bindingBase as PriorityBinding;
            if (priorityBinding != null)
            {
                var result = new PriorityBinding
                {
                    BindingGroupName = priorityBinding.BindingGroupName,
                    FallbackValue = priorityBinding.FallbackValue,
                    StringFormat = priorityBinding.StringFormat,
                    TargetNullValue = priorityBinding.TargetNullValue,
                };

                foreach (var childBinding in priorityBinding.Bindings)
                    result.Bindings.Add(childBinding.Clone());

                return result as T;
            }
#endif
            return null;
        }

        /// <summary>
        /// Searches for a specified visual element in the child tree by name
        /// </summary>
        /// <param name="element">The object to find the child object for. This is expected to be either a <see cref="FrameworkElement"/> or a <see cref="FrameworkContentElement"/>.</param>
        /// <param name="name">The name of the child to find</param>
        /// <returns>The instance of the child with the given name, otherwise null</returns>
        public static FrameworkElement FindVisualChildByName(this DependencyObject element, string name)
        {
            if (element == null)
                return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child != null && child.Name == name)
                    return child;

                var childsChild = child.FindVisualChildByName(name);

                if (childsChild != null)
                    return childsChild;
            }

            return null;
        }

        /// <summary>
        /// Returns all visual childs and sub child (recursively) of the element that matches the given type
        /// </summary>
        /// <typeparam name="T">The typ of child to search for</typeparam>
        /// <param name="element">The parent element</param>
        /// <returns>A collection of <see cref="FrameworkElement"/></returns>
        public static IEnumerable<FrameworkElement> FindVisualChildren<T>(this DependencyObject element)
        {
            List<FrameworkElement> elements = new List<FrameworkElement>();
            FindVisualChildren(typeof(T), element as FrameworkElement, elements);
            return elements;
        }

        /// <summary>
        /// Returns all visual childs and sub child (recursively) of the element that matches the given type
        /// </summary>
        /// <param name="element">The parent element</param>
        /// <param name="dependencyObjectType">The typ of child to search for</param>
        /// <returns>A collection of <see cref="FrameworkElement"/></returns>
        public static IEnumerable FindVisualChildren(this DependencyObject element, Type dependencyObjectType)
        {
            List<FrameworkElement> elements = new List<FrameworkElement>();
            FindVisualChildren(dependencyObjectType, element as FrameworkElement, elements);
            return elements;
        }

        /// <summary>
        /// Returns the parent object of the specified object by processing the visual tree.
        /// </summary>
        /// <param name="element">The object to find the parent object for. This is expected to be either a <see cref="FrameworkElement"/> or a <see cref="FrameworkContentElement"/>.</param>
        /// <param name="dependencyObjectType">The type of the parent to find</param>
        /// <returns>The requested parent object.</returns>
        public static DependencyObject FindVisualParent(this DependencyObject element, Type dependencyObjectType)
        {
            if (element == null)
                return null;

            var parent = VisualTreeHelper.GetParent(element);

            if (parent == null)
                return null;
            if (parent.GetType() == dependencyObjectType)
                return parent;
            else
                return FindVisualParent(parent, dependencyObjectType);
        }

        /// <summary>
        /// Returns the parent object of the specified object by processing the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the parent to find</typeparam>
        /// <param name="element">The object to find the parent object for. This is expected to be either a <see cref="FrameworkElement"/> or a <see cref="FrameworkContentElement"/>.</param>
        /// <returns>The requested parent object.</returns>
        public static T FindVisualParent<T>(this DependencyObject element) where T : DependencyObject =>
            element.FindVisualParent(typeof(T)) as T;

        /// <summary>
        /// Climbs up the visual tree and returns the top most visual parent
        /// </summary>
        /// <param name="element">The object that serves as starting point</param>
        /// <returns>The top most element in the tree</returns>
        public static DependencyObject FindVisualRootElement(this DependencyObject element)
        {
            if (element == null)
                return null;

            var parent = VisualTreeHelper.GetParent(element);

            if (parent == null)
                return element;
            else
                return parent.FindVisualRootElement();
        }

        /// <summary>
        /// Returns all DependendencyProperty fields in UWP and Desktop
        /// </summary>
        /// <param name="dependencyObject">The dependency object whose dependency property to extract</param>
        /// <returns>A collection of dependencyProperties</returns>
        public static IEnumerable<DependencyProperty> GetDependencyProperties(this DependencyObject dependencyObject)
        {
            var type = dependencyObject.GetType();

            foreach (var item in type.GetFieldsEx(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType == typeof(DependencyProperty)))
                yield return item.GetValue(null) as DependencyProperty;

            foreach (var item in type.GetPropertiesEx(BindingFlags.Public | BindingFlags.Static).Where(x => x.PropertyType == typeof(DependencyProperty)))
                yield return item.GetValue(null) as DependencyProperty;
        }

        /// <summary>
        /// Gets the direct visual children of the element
        /// </summary>
        /// <param name="element">The parent element</param>
        /// <returns>A collection of children</returns>
        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject element)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                yield return VisualTreeHelper.GetChild(element, i);
        }

        /// <summary>
        /// Returns the parent object of the specified object by processing the visual tree.
        /// </summary>
        /// <param name="element">The object to find the parent object for. This is expected to be either a <see cref="FrameworkElement"/>.</param>
        /// <returns>The requested parent object.</returns>
        public static DependencyObject GetVisualParent(this DependencyObject element) => VisualTreeHelper.GetParent(element);

        /// <summary>
        /// Handles neccessary setting of the <see cref="IViewModel.IsLoading"/> flag if implemented and the error handling
        /// </summary>
        /// <param name="viewModel">The viewmodel to start the operation from</param>
        /// <param name="action">The action that occures</param>
        /// <returns>true if this <see cref="Action"/> was executed without <see cref="Exception"/>; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="viewModel"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null</exception>
        public static bool Run(this IViewModel viewModel, Action action)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            viewModel.IsLoading = true;

            try
            {
                action();
                return true;
            }
            catch (Exception e)
            {
                viewModel.OnException(e);
                return false;
            }
            finally
            {
#pragma warning disable 4014
                viewModel.Dispatcher.RunAsync(() => viewModel.IsLoading = false);
#pragma warning restore 4014
            }
        }

        /// <summary>
        /// Handles neccessary setting of the <see cref="IViewModel.IsLoading"/> flag and the error handling
        /// </summary>
        /// <param name="viewModel">The viewmodel to start the operation from</param>
        /// <param name="action">The action that occures</param>
        /// <returns>true if this <see cref="Action"/> was executed without <see cref="Exception"/>; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="viewModel"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null</exception>
        public static async Task<bool> RunAsync(this IViewModel viewModel, Func<Task> action)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            viewModel.IsLoading = true;

            try
            {
                await action();
                return true;
            }
            catch (Exception e)
            {
                viewModel.OnException(e);
                return false;
            }
            finally
            {
                await viewModel.Dispatcher.RunAsync(() => viewModel.IsLoading = false);
            }
        }

        /// <summary>
        /// Saves the visuals of a <see cref="FrameworkElement"/> to a png file
        /// </summary>
        /// <param name="visual">The <see cref="FrameworkElementFactory"/> whose visuals are drawn to an image</param>
        /// <param name="file">The file of the png to save to</param>
        public static async Task SaveVisualAsPngAsync(this FrameworkElement visual,
#if WINDOWS_UWP
            StorageFile
#else
            FileInfo
#endif

            file)
        {
#if WINDOWS_UWP

            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(visual, (int)visual.Width, (int)visual.Height);
            var pixels = await renderTargetBitmap.GetPixelsAsync();

            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                byte[] bytes = pixels.ToArray();
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                     BitmapAlphaMode.Ignore,
                                     (uint)visual.Width, (uint)visual.Height,
                                     96, 96, bytes);

                await encoder.FlushAsync();
            }
#else
            await Task.FromResult(0); // HACK

            var encoder = new PngBitmapEncoder();
            var bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);

            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = file.Create())
            {
                encoder.Save(stream);
            }

#endif
        }

        /// <summary>
        /// Creates a new instance of <see cref="BitmapImage"/> and assigns the <see cref="byte"/> array to its <see cref="BitmapImage.StreamSource"/> property
        /// </summary>
        /// <param name="bytes">The array of bytes that represents the image</param>
        /// <returns>A new instance of <see cref="BitmapImage"/></returns>
        public static async Task<BitmapImage> ToBitmapImageAsync(this byte[] bytes)
        {
            if (bytes != null && bytes.Length > 0)
            {
                var image = new BitmapImage();
#if WINDOWS_UWP
                using (var stream = new InMemoryRandomAccessStream())
                {
                    await stream.WriteAsync(bytes.AsBuffer());

                    stream.Seek(0);
                    image.SetSource(stream);
                }
#else
                // HACK
                await Task.FromResult(0);
                image.BeginInit();
                image.StreamSource = new MemoryStream(bytes);
                image.EndInit();
                image.Freeze();
#endif

                return image;
            }

            return null;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BitmapImage"/> and assigns the <see cref="Stream"/> to its <see cref="BitmapImage.StreamSource"/> property
        /// <para/>
        /// Returns null if <paramref name="stream"/> is null.
        /// </summary>
        /// <param name="stream">The stream that contains an image</param>
        /// <returns>A new instance of <see cref="BitmapImage"/></returns>
        public static async Task<BitmapImage> ToBitmapImageAsync(this Stream stream)
        {
            if (stream == null)
                return null;

            try
            {
                var image = new BitmapImage();
#if WINDOWS_UWP

                var randomAccessStream = await stream.ToRandomAccessStreamAsync();
                randomAccessStream.Seek(0);

                image.SetSource(randomAccessStream);
#else

                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
#endif
                await Task.Run(() => { });
                return image;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                stream.Dispose();
            }
        }

        /// <summary>
        /// Tries to close a view associated with the view model
        /// </summary>
        /// <param name="viewModel">The viewmodel to that was assigned to the window's data context</param>
        /// <returns>Returns true if successfully closed, otherwise false</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="viewModel"/> is null</exception>
        public static bool TryClose(this IDialogViewModel viewModel) => Navigator.Current.TryClose(viewModel);

        private static void FindVisualChildren(Type childType, FrameworkElement element, List<FrameworkElement> list)
        {
            if (element != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                    if (child != null && child.GetType() == childType)
                        list.Add(child);

                    if (child != null)
                        FindVisualChildren(childType, child, list);
                }
            }
        }
    }
}