using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cauldron.Attached
{
    /// <summary>
    /// Provides Attached Properties to enable localization in controls.
    /// <para/>
    /// Existing text are overridden.
    /// <para />
    /// Supported controls: <see cref="ToolTipService.ToolTipProperty"/> <see cref="TextBlock.Text"/>, <see cref="ContentControl.Content"/>
    /// </summary>
    public static partial class Localized
    {
        private static void OnTooltipChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var text = AssignText(dependencyObject, args.NewValue as string);
            ToolTipService.SetToolTip(dependencyObject, text);
        }
    }
}