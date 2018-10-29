using Newtonsoft.Json;
using System;
using System.Windows.Media;

namespace Cauldron.JsonConverters
{
    /// <summary>
    /// Converts a <see cref="string"/> representing an ARGB hex to and from JSON as <see cref="SolidColorBrush"/>.
    /// </summary>
    public sealed class ColorHexToSolidColorBrush : JsonConverter
    {
        /// <exclude/>
        public override bool CanConvert(Type objectType) => objectType == typeof(Brush) || objectType == typeof(SolidColorBrush);

        /// <exclude/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value?.ToString();

            if (string.IsNullOrEmpty(value))
                return Brushes.Transparent;

            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(value[0] == '#' ? value : "#" + value));
        }

        /// <exclude/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            void SetColor(Color color)
            {
                writer.WriteValue($"#{color.A.ToString("X2")}{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}");
            }

            if (value is SolidColorBrush solidColorBrush)
                SetColor(solidColorBrush.Color);
            else
                SetColor(Colors.Transparent);
        }
    }
}