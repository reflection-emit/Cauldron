using Cauldron.Activator;
using Cauldron.Core.Reflection;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cauldron.XAML
{
    internal static class Common
    {
        public static void AddTransistionStoryboard(FrameworkElement view)
        {
            view.RenderTransformOrigin = new Point(0.5, 0.5);
            view.RenderTransform = new ScaleTransform(0.4, 0.4);
            view.Opacity = 0;

            view.Loaded += (s, e) =>
            {
                var viewStoryboard = new Storyboard();
                var scaleXAnimation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromMilliseconds(110)));
                var scaleYAnimation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromMilliseconds(110)));
                var opacityAnimation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromMilliseconds(200)));
                Storyboard.SetTarget(viewStoryboard, view);
                Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
                Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(FrameworkElement.OpacityProperty));
                viewStoryboard.Children.Add(scaleXAnimation);
                viewStoryboard.Children.Add(scaleYAnimation);
                viewStoryboard.Children.Add(opacityAnimation);
                viewStoryboard.AutoReverse = false;
                viewStoryboard.Begin();
            };
        }

        public static Window CreateWindow(ref WindowType windowType)
        {
            if (windowType == null && Factory.HasContract(typeof(Window)))
                windowType = new WindowType { IsFactoryType = true };

            if (windowType == null)
                windowType = new WindowType { Type = Assemblies.ExportedTypes.FirstOrDefault(x => x.IsSubclassOf(typeof(Window))) };

            if (windowType == null)
                windowType = new WindowType { Type = typeof(Window) };

            return windowType.CreateWindow();
        }
    }
}