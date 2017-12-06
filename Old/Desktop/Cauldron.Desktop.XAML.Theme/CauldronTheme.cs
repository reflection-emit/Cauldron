using System.Windows.Media;

namespace Cauldron.XAML.Theme
{
    public static class CauldronTheme
    {
        public const string AccentBrush = "ThemeAccentBrush";
        public const string AccentColor = "ThemeAccentColor";
        public const string BackgroundBrush = "ThemeBackgroundBrush";
        public const string BackgroundColor = "ThemeBackgroundColor";
        public const string ButtonbackBrush = "ThemeButtonbackBrush";
        public const string ButtonbackColor = "ThemeButtonbackColor";
        public const string ComboBoxArrowBrush = "ThemeComboBoxArrowBrush";
        public const string ComboBoxArrowColor = "ThemeComboBoxArrowColor";
        public const string DarkBackgroundBrush = "ThemeDarkBackgroundBrush";
        public const string DarkBackgroundColor = "ThemeDarkBackgroundColor";
        public const string DarkOverlayBrush = "ThemeDarkOverlayBrush";
        public const string DarkOverlayColor = "ThemeDarkOverlayColor";
        public const string DisabledBackgroundBrush = "ThemeDisabledBackgroundBrush";
        public const string DisabledBackgroundColor = "ThemeDisabledBackgroundColor";
        public const string DisabledTextBrush = "ThemeDisabledTextBrush";
        public const string DisabledTextColor = "ThemeDisabledTextColor";
        public const string DropdownBackgroundBrush = "ThemeDropdownBackgroundBrush";
        public const string DropdownBackgroundColor = "ThemeDropdownBackgroundColor";
        public const string HoveredTextBrush = "ThemeHoveredTextBrush";
        public const string HoveredTextColor = "ThemeHoveredTextColor";
        public const string HoverLightBrush = "ThemeHoverLightBrush";
        public const string HoverLightColor = "ThemeHoverLightColor";
        public const string LightAccentBrush = "ThemeLightAccentBrush";
        public const string LightBackgroundBrush = "ThemeLightBackgroundBrush";
        public const string LightBackgroundColor = "ThemeLightBackgroundColor";
        public const string LightBorderBrush = "ThemeLightBorderBrush";
        public const string LightBorderColor = "ThemeLightBorderColor";
        public const string LightOverlayBrush = "ThemeLightOverlayBrush";
        public const string LightOverlayColor = "ThemeLightOverlayColor";
        public const string TextBrush = "ThemeTextBrush";
        public const string TextColor = "ThemeTextColor";

        public static void SetAccentColor(Color color)
        {
            if (ApplicationBase.Current.Resources.Contains(AccentColor))
                ApplicationBase.Current.Resources[AccentColor] = color;
            else
                ApplicationBase.Current.Resources.Add(AccentColor, color);
        }
    }
}