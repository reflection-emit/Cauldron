using System;
using System.Windows;
using System.Windows.Controls;

namespace Cauldron.XAML.Theme
{
    internal sealed class InlineTextValueConverter : ValueConverterBase
    {
        public override object OnConvert(object value, Type targetType, object parameter, string language)
        {
            var text = value as string;

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (text.Contains("<Inline>") && text.Contains("</Inline>"))
            {
                // We can have multiple inlined localized texts in the string... We have to remove
                // them all
                text = "<Inline>" + text.Replace("<Inline>", "").Replace("</Inline>", "") + "</Inline>";

                var textBlock = new TextBlock();
                textBlock.Inlines.Clear();
                textBlock.Inlines.Add(XAMLHelper.ParseToInline(text));
                textBlock.VerticalAlignment = VerticalAlignment.Stretch;
                textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.TextTrimming = TextTrimming.None;
                textBlock.TextAlignment = TextAlignment.Justify;
                textBlock.MaxWidth = 350;

                return textBlock;
            }

            return text;
        }

        public override object OnConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}