using Newtonsoft.Json;
using System;

namespace Cauldron.JsonConverters
{
    /// <summary>
    /// Converts a <see cref="long"/> representing minutes to and from JSON as <see cref="TimeSpan"/>.
    /// </summary>
    public sealed class MinutesToTimeSpan : JsonConverter
    {
        /// <exclude/>
        public override bool CanConvert(Type objectType) => objectType == typeof(TimeSpan);

        /// <exclude/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            TimeSpan.FromMinutes(reader.Value.ToString().ToDouble(serializer.Culture.NumberFormat));

        /// <exclude/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
           writer.WriteValue(((TimeSpan)value).TotalMinutes);
    }
}