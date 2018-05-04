using Cauldron.Activator;
using Cauldron.Core.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Cauldron.Localization
{
    /// <summary>
    /// Provides methods regarding localization.
    /// <para/>
    /// https://github.com/Capgemini/Cauldron/wiki/Localization
    /// </summary>
    [Component(typeof(Locale), FactoryCreationPolicy.Singleton)]
    public sealed class Locale : Factory<Locale>
    {
        private const string LocalizationSource = "Cauldron.Localization.ILocalizationSource";
        private CultureInfo cultureInfo;
        private Dictionary<string, ILocalizationKeyValue> source = new Dictionary<string, ILocalizationKeyValue>();

        /// <exclude/>
        [ComponentConstructor]
        public Locale()
        {
            Assemblies.LoadedAssemblyChanged += (s, e) =>
            {
                if (e.Cauldron == null)
                    return;

                var factoryCache = e.Cauldron as IFactoryCache;

                if (factoryCache == null)
                    return;

                var newLocalizationSources = factoryCache
                      .GetComponents()
                      .Where(x => x.ContractName.GetHashCode() == LocalizationSource.GetHashCode() && x.ContractName == LocalizationSource).Select(x => x.CreateInstance() as ILocalizationSource)
                      .ToArray();

                foreach (var item in newLocalizationSources.SelectMany(x => x.GetValues()))
                {
                    var key = item.Key;

                    if (this.source.ContainsKey(key))
                        continue;

                    this.source.Add(key, item);
                }
            };

            foreach (var item in Factory.CreateMany<ILocalizationSource>()?.SelectMany(x => x.GetValues()))
            {
                var key = item.Key;

                if (this.source.ContainsKey(key))
                    continue;

                this.source.Add(key, item);
            }

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
        /// Gets or sets a value that indicates if a localization for the given key is missing.
        /// </summary>
        public char MissingLocalizationIndicator { get; set; } = '♦';

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

                if (this.source.TryGetValue(key, out ILocalizationKeyValue result))
                    return result.GetValue(this.CultureInfo.TwoLetterISOLanguageName);

                return key + " " + this.MissingLocalizationIndicator; // indicates that the localization was not provided. Someone has to do his homework
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

                    if (long.TryParse(value?.ToString(), out long enumValue) && enumValue < 0)
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
#if WINDOWS_UWP
            return new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);
#else
            return CultureInfo.CurrentCulture;
#endif
        }

        /// <summary>
        /// Determines whether the <see cref="Locale"/> contains a localized string defined by the <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the localized string</param>
        /// <returns>true if <see cref="Locale"/> contains the string; otherwise false.</returns>
        public bool Contains(string key) => this.source.ContainsKey(key);
    }
}