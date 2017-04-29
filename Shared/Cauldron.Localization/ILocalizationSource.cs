using System.Collections.Generic;

namespace Cauldron.Localization
{
    /// <summary>
    /// Represents a source for a localization source
    /// </summary>
    public interface ILocalizationSource
    {
        /// <summary>
        /// Returns all key values pair of the localization source
        /// </summary>
        /// <returns>A collection of key values pair</returns>
        IEnumerable<ILocalizationKeyValue> GetValues();
    }
}