using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Extensions;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Cauldron.Localization
{
    /// <summary>
    /// Provides methods regarding localization
    /// </summary>
    [Component(typeof(Locale), FactoryCreationPolicy.Singleton)]
    public sealed class Locale : Singleton<Locale>
    {
        private CultureInfo cultureInfo;
        private ILocalizationSource[] source;

        /// <summary>
        /// Initiates a new instance of the <see cref="Locale"/> class
        /// </summary>
        [ComponentConstructor]
        private Locale()
        {
            if (!Factory.HasContract(typeof(ILocalizationSource)))
                throw new Exception("There is no valid implementation of 'ILocalizationSource' found");

            this.Rebuild();

#if WINDOWS_UWP
            this.CultureInfo = new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);
#else
            this.CultureInfo = CultureInfo.CurrentCulture;
#endif
        }

        /// <summary>
        /// Gets or sets the culture used for the localization. Default is <see cref="CultureInfo.CurrentCulture"/> in Windows desktop and the UI language in UWP
        /// </summary>
        public CultureInfo CultureInfo
        {
            get { return this.cultureInfo; }
            set
            {
                this.cultureInfo = value;
#if !WINDOWS_UWP && !NETCORE
                this.cultureInfo.ClearCachedData();
#endif
            }
        }

        /// <summary>
        /// Gets the localized string with the specified key
        /// <para />
        /// Returns null if the key was not found
        /// </summary>
        /// <param name="key">The key of the localized string</param>
        /// <returns>The localized string</returns>
        public string this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    return string.Empty;

                for (int i = 0; i < this.source.Length; i++)
                    if (this.source[i].Contains(key, this.CultureInfo.TwoLetterISOLanguageName))
                        return this.source[i].GetValue(key, this.CultureInfo.TwoLetterISOLanguageName);

                return key + "*"; // indicates that the localization was not provided. Someone has to do his homework
            }
        }

        /// <summary>
        /// Gets the localized string with an object as a key
        /// <para/>
        /// If the <paramref name="key"/> is an enum the returned formatting will be: enum value - enum name
        /// <para/>
        /// <see cref="long"/>, <see cref="int"/>, <see cref="uint"/> and <see cref="ulong"/> are formatted using {0:#,#}.
        /// <para/>
        /// <see cref="double"/>, <see cref="float"/> and <see cref="decimal"/> are formatted using {0:#,#.00}.
        /// <para/>
        /// Otherwise its tries to retrieve the localized string using the <paramref name="key"/>'s type name as key.
        /// </summary>
        /// <param name="key">The object used as key</param>
        /// <returns>The localized string</returns>
        public string this[object key]
        {
            get
            {
                if (key == null)
                    return null;

                if (key is string)
                    return this[key as string];

                var type = key.GetType();

#if WINDOWS_UWP || NETCORE
                if (type.GetTypeInfo().IsEnum)
#else
                if (type.IsEnum)
#endif
                {
                    var value = Convert.ChangeType(key, Enum.GetUnderlyingType(type));

                    if (value.ToLong() < 0)
                        return string.Empty; // We dont show values under zero

                    var text = this[key?.ToString()];
                    if (text == null)
                        text = key.ToString();
                    return $"{value} - {text}";
                }

                if (type == typeof(DateTime))
                {
                    var dateTime = (DateTime)key;

                    if (dateTime.Hour == 0 && dateTime.Minute == 0 && dateTime.Second == 0 && dateTime.Millisecond == 0)
                        return ((DateTime)key).ToString("d", this.CultureInfo);
                    else
                        return ((DateTime)key).ToString("G", this.CultureInfo);
                }

                if (type == typeof(long) || type == typeof(int) || type == typeof(ulong) || type == typeof(uint))
                    return string.Format(this.CultureInfo.NumberFormat, "{0:#,0}", key);

                if (type == typeof(double) || type == typeof(float) || type == typeof(decimal))
                    return string.Format(this.CultureInfo.NumberFormat, "{0:#,0.00}", key);

                return this[type.Name];
            }
        }

        /// <summary>
        /// Gets the current culture
        /// </summary>
        /// <returns></returns>
        public static CultureInfo GetCurrentCultureInfo()
        {
            if (Factory.HasContract(typeof(ILocalizationSource)))
                return Locale.Current.CultureInfo;

#if WINDOWS_UWP
            return new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);
#else
            return CultureInfo.CurrentCulture;
#endif
        }

        /// <summary>
        /// Rebuilds the localization source.
        /// </summary>
        public void Rebuild()
        {
            this.source = Factory.CreateMany<ILocalizationSource>()?.ToArray();
        }
    }
}