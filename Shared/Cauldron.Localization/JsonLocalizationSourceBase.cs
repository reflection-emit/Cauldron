using Cauldron.Reflection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Cauldron;

namespace Cauldron.Localization
{
    /// <summary>
    /// Represents a base class for a localization using embedded json files.
    /// <code>
    /// The json should have the following structure:
    ///
    /// [
    ///     {
    ///         "key" : "attention-message",
    ///         "en" : "Don't press the button.",
    ///         "ja" : "ボタンを押さないでください。"
    ///     },
    ///     {
    ///         "key" : "message2",
    ///         "en" : "I am groot",
    ///         "ja" : "私はホートです"
    ///     }
    /// ]
    /// </code>
    /// </summary>
    public abstract class JsonLocalizationSourceBase<T> : ILocalizationSource where T : ILocalizationKeyValue
    {
        private IEnumerable<ILocalizationKeyValue> localizations;

        /// <summary>
        /// Initializes a new instance of <see cref="JsonLocalizationSourceBase{T}"/>
        /// </summary>
        /// <param name="filename">The filename of the json file the contains the localization.</param>
        public JsonLocalizationSourceBase(string filename) =>
            this.localizations = JsonConvert.DeserializeObject<List<T>>(Assemblies.GetManifestResource(filename).TryEncode()).Cast<ILocalizationKeyValue>().ToArray();

        /// <summary>
        /// Returns all key values pair of the localization source
        /// </summary>
        /// <returns>A collection of key values pair</returns>
        public IEnumerable<ILocalizationKeyValue> GetValues() => this.localizations;
    }
}