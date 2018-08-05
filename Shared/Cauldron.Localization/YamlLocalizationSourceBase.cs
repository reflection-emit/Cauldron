using Cauldron.Reflection;
using Cauldron.Yaml;
using System.Collections.Generic;

namespace Cauldron.Localization
{
    /// <summary>
    /// Represents a base class for a localization using embedded yaml files.
    /// <code>
    /// The yaml should have the following structure:
    ///
    /// Attention-message
    ///     en: Don't press the button.
    ///     ja: ボタンを押さないでください。
    /// Message2
    ///     en: I am groot
    ///     ja: 私はホートです
    /// </code>
    /// </summary>
    public abstract class YamlLocalizationSourceBase<T> : ILocalizationSource where T : class, ILocalizationKeyValue, new()
    {
        private T[] localizations;

        /// <summary>
        /// Initializes a new instance of <see cref="YamlLocalizationSourceBase{T}"/>
        /// </summary>
        /// <param name="filename">The filename of the json file the contains the localization.</param>
        public YamlLocalizationSourceBase(string filename) =>
            this.localizations = YamlConvert.DeserializeObject<T>(Assemblies.GetManifestResource(filename).TryEncode());

        /// <summary>
        /// Returns all key values pair of the localization source
        /// </summary>
        /// <returns>A collection of key values pair</returns>
        public IEnumerable<ILocalizationKeyValue> GetValues() => this.localizations;
    }
}