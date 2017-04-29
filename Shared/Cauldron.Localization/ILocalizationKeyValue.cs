namespace Cauldron.Localization
{
    /// <summary>
    /// Represents an interface of the <see cref="ILocalizationSource"/> dictionary item
    /// </summary>
    public interface ILocalizationKeyValue
    {
        /// <summary>
        /// Gets the localization key
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the localized value of the key
        /// </summary>
        /// <param name="twoLetterISOLanguageName">The two letter iso language name according to ISO</param>
        /// <returns>The localized string.</returns>
        string GetValue(string twoLetterISOLanguageName);
    }
}