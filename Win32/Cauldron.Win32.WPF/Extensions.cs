using System;
using System.Threading.Tasks;
using System.IO;
using Cauldron.XAML.ViewModels;
using Cauldron.XAML.Navigation;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using Cauldron.XAML.Threading;

#if WINDOWS_UWP

using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Data;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

#else

using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

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
        /// <param name="element">The object to find the child object for. This is expected to be either a <see cref="FrameworkElement"/> or a FrameworkContentElement.</param>
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
        /// <returns>A collection of <typeparamref name="T"/></returns>
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject element) where T : FrameworkElement
        {
            var childType = typeof(T);
            var elements = new List<T>();
            FindVisualChildren(x => x.GetType() == childType, element as FrameworkElement, false, elements);
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
            var elements = new List<FrameworkElement>();
            FindVisualChildren(x => x.GetType() == dependencyObjectType, element as FrameworkElement, false, elements);
            return elements;
        }

        /// <summary>
        /// Returns all visual childs and sub child (recursively) of the element that matches the predicate.
        /// </summary>
        /// <param name="element">The parent element</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A collection of <see cref="FrameworkElement"/></returns>
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject element, Func<FrameworkElement, bool> predicate) where T : FrameworkElement
        {
            var elements = new List<T>();
            FindVisualChildren(predicate, element as FrameworkElement, true, elements);
            return elements;
        }

        /// <summary>
        /// Returns the parent object of the specified object by processing the visual tree.
        /// </summary>
        /// <param name="element">The object to find the parent object for. This is expected to be either a <see cref="FrameworkElement"/> or a FrameworkContentElement.</param>
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
        /// The difference to <see cref="FindVisualParent(DependencyObject, Type)"/> is that this also consider sub-classes.
        /// <para/>
        ///
        /// </summary>
        /// <typeparam name="T">The type of the parent to find</typeparam>
        /// <param name="element">The object to find the parent object for. This is expected to be either a <see cref="FrameworkElement"/> or a FrameworkContentElement.</param>
        /// <returns>The requested parent object.</returns>
        public static T FindVisualParent<T>(this DependencyObject element) where T : DependencyObject
        {
            if (element == null)
                return null;

            var parent = VisualTreeHelper.GetParent(element);

            if (parent == null)
                return null;
            if (parent is T result)
                return result;
            else
                return FindVisualParent<T>(parent);
        }

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
        public static IEnumerable<DependencyPropertyInfo> GetDependencyProperties(this DependencyObject dependencyObject)
        {
            var type = dependencyObject.GetType();

            foreach (var item in type.GetFieldsEx(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType == typeof(DependencyProperty)))
                yield return new DependencyPropertyInfo(item.GetValue(null) as DependencyProperty, item.Name);

            foreach (var item in type.GetPropertiesEx(BindingFlags.Public | BindingFlags.Static).Where(x => x.PropertyType == typeof(DependencyProperty)))
                yield return new DependencyPropertyInfo(item.GetValue(null) as DependencyProperty, item.Name);
        }

        /// <summary>
        /// Get the value of the source of the given <see cref="BindingExpression"/>.
        /// </summary>
        /// <param name="bindingExpression">The <see cref="BindingExpression"/> of interest.</param>
        /// <returns>The value of the <see cref="DependencyProperty"/>.</returns>
        public static object GetValue(this BindingExpression bindingExpression) => XAMLHelper.GetPropertyValueFromPath(bindingExpression.DataItem, bindingExpression.ParentBinding.Path.Path);

        /// <summary>
        /// Get the value of the source of the given <see cref="Binding"/>.
        /// </summary>
        /// <param name="binding">The <see cref="Binding"/> of interest.</param>
        /// <returns>The value of the <see cref="DependencyProperty"/>.</returns>
        public static object GetValue(this Binding binding) => XAMLHelper.GetPropertyValueFromPath(binding.Source, binding.Path.Path);

        /// <summary>
        /// Gets the value of the <see cref="FrameworkElement"/>'s <see cref="DependencyProperty"/>. If the <see cref="DependencyProperty"/> is binded, then it will
        /// return the value of the binding source. If it is not binded, then it will return the value of the <see cref="DependencyProperty"/>.
        /// </summary>
        /// <param name="frameworkElement">The <see cref="FrameworkElement"/> to get the value of the <see cref="DependencyProperty"/> from.</param>
        /// <param name="dependencyProperty">The <see cref="DependencyProperty"/> of interest.</param>
        /// <returns>The value of the <see cref="DependencyProperty"/>.</returns>
        public static object GetValueFromBindingOrProperty(this FrameworkElement frameworkElement, DependencyProperty dependencyProperty)
        {
#if WINDOWS_UWP
            var bindingExpression = frameworkElement.GetBindingExpression(dependencyProperty);
#else
            var bindingExpression = BindingOperations.GetBindingExpression(frameworkElement, dependencyProperty);
#endif
            if (bindingExpression == null || bindingExpression.ParentBinding == null)
                return frameworkElement.GetValue(dependencyProperty);

            return bindingExpression.GetValue();
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
                viewModel.IsLoading = false;
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
            var awaiterTask = new Task(() => { });
            var result = false;

            await Task.Run(async () =>
            {
                try
                {
                    await action();
                    result = true;
                }
                catch (Exception e)
                {
                    viewModel.OnException(e);
                    result = false;
                }
                finally
                {
                    viewModel.IsLoading = false;
                    awaiterTask.Start();
                }
            });

            Task.WaitAll(awaiterTask);
            return result;
        }

        /// <summary>
        /// Runs the <paramref name="action"/> asyncronously using the <see cref="IViewModel.Dispatcher"/> on the lowest priority.
        /// Handles neccessary setting of the <see cref="IViewModel.IsLoading"/> flag and the error handling.
        /// </summary>
        /// <param name="viewModel">The viewmodel to start the operation from</param>
        /// <param name="action">The action that occures</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="viewModel"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null</exception>
        public static async Task RunDispatcherAsync(this IViewModel viewModel, Func<Task> action)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            viewModel.IsLoading = true;

            try
            {
                await viewModel.Dispatcher.RunAsync(DispatcherPriority.Low, () => action());
            }
            catch (Exception e)
            {
                viewModel.OnException(e);
            }
            finally
            {
                viewModel.IsLoading = false;
            }
        }

        /// <summary>
        /// Saves the visuals of a <see cref="FrameworkElement"/> to a png file
        /// </summary>
        /// <param name="visual">The FrameworkElementFactory whose visuals are drawn to an image</param>
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
        /// Attaches a binding to a FrameworkElement, using the provided binding object.
        /// </summary>
        /// <param name="element">The <see cref="FrameworkElement"/> that is binded to</param>
        /// <param name="dp">The dependency property identifier of the property that is data bound.</param>
        /// <param name="source">The data source for the binding.</param>
        /// <param name="path">The path to the binding source property.</param>
        /// <param name="mode">Indicates the direction of the data flow in the binding.</param>
        /// <returns>The binding that is used for the property.</returns>
        public static Binding SetBinding(this FrameworkElement element, DependencyProperty dp, object source, PropertyPath path, BindingMode mode)
        {
            var binding = new Binding
            {
                Path = path,
                Source = source,
                Mode = mode
            };
            element.SetBinding(dp, binding);
            return binding;
        }

        /// <summary>
        /// Set the inlined content of the <see cref="TextBlock"/>. This will clear the previous inline elements.
        /// If <paramref name="text"/> is null then previous inline will also be cleared.
        /// </summary>
        /// <param name="textBlock">The <see cref="TextBlock"/> to set the inline elements to.</param>
        /// <param name="text">The XAML formated text.</param>
        public static void SetInlinedText(this TextBlock textBlock, string text)
        {
            if (text == null)
            {
                textBlock.Inlines.Clear();
                return;
            }

            if (text.StartsWith("<Inline>") && text.EndsWith("</Inline>"))
            {
                textBlock.Inlines.Clear();
                textBlock.Inlines.Add(XAMLHelper.ParseToInline(text));
            }
            else
                textBlock.Text = text;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BitmapImage"/> and assigns the <see cref="byte"/> array to its BitmapImage.StreamSource property
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
        /// Creates a new instance of <see cref="BitmapImage"/> and assigns the <see cref="Stream"/> to its BitmapImage.StreamSource property
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

        private static void FindVisualChildren<T>(Func<FrameworkElement, bool> predicate, FrameworkElement element, bool skipChildren, List<T> list) where T : FrameworkElement
        {
            if (element != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    var child = VisualTreeHelper.GetChild(element, i) as T;

                    if (child != null && predicate(child))
                        list.Add(child);
                    else if (skipChildren)
                        continue;

                    if (child != null)
                        FindVisualChildren(predicate, child, skipChildren, list);
                }
            }
        }
    }
}