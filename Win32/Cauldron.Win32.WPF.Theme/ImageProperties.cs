using Cauldron.Activator;
using Cauldron.Core.Threading;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Cauldron.XAML.Theme
{
    /// <summary>
    /// Provides attached properties for the <see cref="Image"/> control
    /// </summary>
    public static class ImageProperties
    {
        #region Dependency Attached Property AnimateOpacity

        /// <summary>
        /// Identifies the AnimateOpacity dependency property
        /// </summary>
        public static readonly DependencyProperty AnimateOpacityProperty = DependencyProperty.RegisterAttached("AnimateOpacity", typeof(bool), typeof(ImageProperties), new PropertyMetadata(false));

        /// <summary>
        /// Gets the value of AnimateOpacity
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static bool GetAnimateOpacity(DependencyObject obj)
        {
            return (bool)obj.GetValue(AnimateOpacityProperty);
        }

        /// <summary>
        /// Sets the value of the AnimateOpacity attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetAnimateOpacity(DependencyObject obj, bool value)
        {
            obj.SetValue(AnimateOpacityProperty, value);
        }

        #endregion Dependency Attached Property AnimateOpacity

        #region Dependency Attached Property ImageKey

        /// <summary>
        /// Identifies the ImageKey dependency property
        /// </summary>
        public static readonly DependencyProperty ImageKeyProperty = DependencyProperty.RegisterAttached("ImageKey", typeof(string), typeof(ImageProperties), new PropertyMetadata(null, OnImageKeyChanged));

        /// <summary>
        /// Gets the value of ImageKey
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetImageKey(DependencyObject obj)
        {
            return (string)obj.GetValue(ImageKeyProperty);
        }

        /// <summary>
        /// Sets the value of the ImageKey attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetImageKey(DependencyObject obj, string value)
        {
            obj.SetValue(ImageKeyProperty, value);
        }

        private static async void OnImageKeyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var image = dependencyObject as Image;
            var name = args.NewValue as string;

            if (image == null)
                throw new NotSupportedException($"This attached property can only be attached to a {typeof(Image).FullName}");

            if (string.IsNullOrEmpty(name))
            {
                image.Source = null;
                return;
            }

            Storyboard opacityStoryboard = null;

            if (GetAnimateOpacity(image))
            {
                image.Opacity = 0;

                opacityStoryboard = new Storyboard();
                Storyboard.SetTarget(opacityStoryboard, image);
#if WINDOWS_UWP
                Storyboard.SetTargetProperty(opacityStoryboard, "Opacity");
                var opacityAnimation = new DoubleAnimation() { To = 1.0, Duration = new Duration(TimeSpan.FromMilliseconds(230)) };
#else
                Storyboard.SetTargetProperty(opacityStoryboard, new PropertyPath(Image.OpacityProperty));
                var opacityAnimation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromMilliseconds(230)));

#endif
                opacityStoryboard.Children.Add(opacityAnimation);
                opacityStoryboard.AutoReverse = false;
            }

            var bitmapImage = await ImageManager.Current.GetImageAsync(args.NewValue as string);
            await Factory.Create<IDispatcher>().RunAsync(DispatcherPriority.High, () =>
            {
                image.Source = bitmapImage;
                opacityStoryboard?.Begin();
            });
        }

        #endregion Dependency Attached Property ImageKey
    }
}