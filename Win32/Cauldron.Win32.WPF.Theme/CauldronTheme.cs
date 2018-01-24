using System.Windows.Media;

namespace Cauldron.XAML.Theme
{
    /// <summary>
    /// Provides constants and method regarding the theming resource color names and method to change the accent color.
    /// </summary>
    public static class CauldronTheme
    {
        /// <exclude/>
        public const string AccentBrush = "ThemeAccentBrush";

        /// <exclude/>
        public const string AccentColor = "ThemeAccentColor";

        /// <exclude/>
        public const string BackgroundBrush = "ThemeBackgroundBrush";

        /// <exclude/>
        public const string BackgroundColor = "ThemeBackgroundColor";

        /// <exclude/>
        public const string ButtonbackBrush = "ThemeButtonbackBrush";

        /// <exclude/>
        public const string ButtonbackColor = "ThemeButtonbackColor";

        /// <exclude/>
        public const string ComboBoxArrowBrush = "ThemeComboBoxArrowBrush";

        /// <exclude/>
        public const string ComboBoxArrowColor = "ThemeComboBoxArrowColor";

        /// <exclude/>
        public const string DarkBackgroundBrush = "ThemeDarkBackgroundBrush";

        /// <exclude/>
        public const string DarkBackgroundColor = "ThemeDarkBackgroundColor";

        /// <exclude/>
        public const string DarkOverlayBrush = "ThemeDarkOverlayBrush";

        /// <exclude/>
        public const string DarkOverlayColor = "ThemeDarkOverlayColor";

        /// <exclude/>
        public const string DisabledBackgroundBrush = "ThemeDisabledBackgroundBrush";

        /// <exclude/>
        public const string DisabledBackgroundColor = "ThemeDisabledBackgroundColor";

        /// <exclude/>
        public const string DisabledTextBrush = "ThemeDisabledTextBrush";

        /// <exclude/>
        public const string DisabledTextColor = "ThemeDisabledTextColor";

        /// <exclude/>
        public const string DropdownBackgroundBrush = "ThemeDropdownBackgroundBrush";

        /// <exclude/>
        public const string DropdownBackgroundColor = "ThemeDropdownBackgroundColor";

        /// <exclude/>
        public const string HoveredTextBrush = "ThemeHoveredTextBrush";

        /// <exclude/>
        public const string HoveredTextColor = "ThemeHoveredTextColor";

        /// <exclude/>
        public const string HoverLightBrush = "ThemeHoverLightBrush";

        /// <exclude/>
        public const string HoverLightColor = "ThemeHoverLightColor";

        /// <exclude/>
        public const string LightAccentBrush = "ThemeLightAccentBrush";

        /// <exclude/>
        public const string LightBackgroundBrush = "ThemeLightBackgroundBrush";

        /// <exclude/>
        public const string LightBackgroundColor = "ThemeLightBackgroundColor";

        /// <exclude/>
        public const string LightBorderBrush = "ThemeLightBorderBrush";

        /// <exclude/>
        public const string LightBorderColor = "ThemeLightBorderColor";

        /// <exclude/>
        public const string LightOverlayBrush = "ThemeLightOverlayBrush";

        /// <exclude/>
        public const string LightOverlayColor = "ThemeLightOverlayColor";

        /// <exclude/>
        public const string TextBrush = "ThemeTextBrush";

        /// <exclude/>
        public const string TextColor = "ThemeTextColor";

        /// <summary>
        /// Changes the accent color of the theme.
        /// <para/>
        /// This should be changed before the views, styles and templates are loaded due to the fact that this theming avoid the using of DynamicResource
        /// for performance reasons.
        /// </summary>
        /// <param name="color"></param>
        public static void SetAccentColor(Color color)
        {
            if (ApplicationBase.Current.Resources.Contains(AccentColor))
                ApplicationBase.Current.Resources[AccentColor] = color;
            else
                ApplicationBase.Current.Resources.Add(AccentColor, color);
        }
    }
}