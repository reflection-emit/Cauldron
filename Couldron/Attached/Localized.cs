using System.Windows;
using System.Windows.Controls;

namespace Couldron.Attached
{
    /// <summary>
    /// Provides Attached Properties to enable localization in controls.
    /// <para/>
    /// Existing text are overridden.
    /// <para />
    /// Supported controls: <see cref="FrameworkElement.ToolTip"/> <see cref="TextBlock.Text"/>, <see cref="ContentControl.Content"/>
    /// </summary>
    public static partial class Localized
    {
        private static void OnTooltipChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var text = AssignText(dependencyObject, args.NewValue as string);

            if (dependencyObject is FrameworkElement)
                (dependencyObject as FrameworkElement).ToolTip = text;
        }
    }
}