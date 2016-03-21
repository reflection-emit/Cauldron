namespace Couldron
{
    /// <summary>
    /// Represents a source for a localization source
    /// </summary>
    public interface ILocalizationSource
    {
        /// <summary>
        /// Determines whether an element is in the <see cref="ILocalizationSource"/>
        /// </summary>
        /// <param name="key">The key of the localized string</param>
        /// <param name="twoLetterISOLanguageName">The two letter iso language name according to ISO</param>
        /// <returns>true if item is found in the <see cref="ILocalizationSource"/>; otherwise, false.</returns>
        bool Contains(string key, string twoLetterISOLanguageName);

        /// <summary>
        /// Returns a localized value associated with the key
        /// </summary>
        /// <param name="key">The key of the localized string data</param>
        /// <param name="twoLetterISOLanguageName">The two letter iso language name according to ISO</param>
        /// <returns>A <see cref="string"/></returns>
        string GetValue(string key, string twoLetterISOLanguageName);
    }
}