using System.Windows;

namespace Couldron.Attached
{
    public static class TextBox
    {
        public static readonly DependencyProperty AlternativeTextKeyProperty = DependencyProperty.RegisterAttached("AlternativeTextKey", typeof(string), typeof(TextBox), new PropertyMetadata("", OnAlternativeTextKeyChanged));
        public static readonly DependencyProperty AlternativeTextLocalizedProperty = DependencyProperty.RegisterAttached("AlternativeTextLocalized", typeof(string), typeof(TextBox), new PropertyMetadata(""));

        public static string GetAlternativeTextKey(DependencyObject obj)
        {
            return (string)obj.GetValue(AlternativeTextKeyProperty);
        }

        public static string GetAlternativeTextLocalized(DependencyObject obj)
        {
            return (string)obj.GetValue(AlternativeTextLocalizedProperty);
        }

        public static void SetAlternativeTextKey(DependencyObject obj, string value)
        {
            obj.SetValue(AlternativeTextKeyProperty, value);
        }

        private static void OnAlternativeTextKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var localization = Factory.Create<Localization>();
            SetAlternativeTextLocalized(d, localization[args.NewValue as string]);
        }

        private static void SetAlternativeTextLocalized(DependencyObject obj, string value)
        {
            obj.SetValue(AlternativeTextLocalizedProperty, value);
        }
    }
}