namespace Cauldron.Yaml
{
    /// <summary>
    /// Provides methods for converting between common language runtime types and YAML types.
    /// </summary>
    public static class YamlConvert
    {
        /// <summary>
        /// Deserializes the YAML to the specified .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The YAML to deserialize.</param>
        /// <returns>The deserialized object from the YAML string.</returns>
        public static T[] DeserializeObject<T>(string value) where T : class, new()
        {
            var yamlDeserializer = new YamlDeserializer();
            return yamlDeserializer.Parse<T>(value);
        }
    }
}