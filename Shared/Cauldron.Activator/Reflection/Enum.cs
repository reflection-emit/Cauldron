using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Cauldron;

namespace System /* Make this prominent ... side by side with Enum */
{
    /// <summary>
    /// Provides static methods for enumerations
    /// </summary>
    public static class EnumEx
    {
        /// <summary>
        /// Returns the <see cref="DisplayNameAttribute.DisplayName"/> of an enum type.
        /// </summary>
        /// <typeparam name="TEnum">The enum type</typeparam>
        /// <returns>A dictionary of display names with the enum value member as key</returns>
        /// <example>
        /// <code>
        /// using System;
        /// using Cauldon.Core;
        ///
        /// public enum TestEnum
        /// {
        ///     [DisplayName("FIRST")]
        ///     One,
        ///     [DisplayName("SECOND")]
        ///     Two,
        ///     [DisplayName("THIRD")]
        ///     Three
        /// }
        ///
        /// public class Program
        /// {
        ///     private static TestEnum GetValue(string value) =&gt;
        ///         MiscUtils
        ///             .GetDisplayNames&lt;TestEnum&gt;()
        ///             .FirstOrDefault(x =&gt; x.Value == value)
        ///             .Key;
        ///
        ///     public static Main(string[] args)
        ///     {
        ///         var value = GetValue("SECOND");
        ///         // Output: Two
        ///         Console.WriteLine(value);
        ///     }
        /// }
        /// </code>
        /// </example>
        public static IReadOnlyDictionary<TEnum, string> GetDisplayNames<TEnum>() where TEnum : struct, IConvertible
        {
            var enumType = typeof(TEnum);

            if (!enumType.GetTypeInfo().IsEnum)
                throw new ArgumentException($"{enumType.FullName} is not an enum type");

            return new ReadOnlyDictionary<TEnum, string>(enumType
                .GetMembers()
#if !WINDOWS_UWP
                .Where(x => x.MemberType == MemberTypes.Field)
#endif
                .Select(x => new { Attribute = x.GetCustomAttribute<DisplayNameAttribute>(), x.Name })
                .Where(x => x.Attribute != null)
                .ToDictionary(x => (TEnum)System.Enum.Parse(enumType, x.Name), x => x.Attribute.DisplayName));
        }

        internal static string GetDisplayName<TEnum>(this TEnum value) where TEnum : struct, IConvertible
        {
            var enumType = typeof(TEnum);

            if (!enumType.GetTypeInfo().IsEnum)
                throw new ArgumentException($"{enumType.FullName} is not an enum type");

            return enumType
                 .GetMembers()
#if !WINDOWS_UWP
                .Where(x => x.MemberType == MemberTypes.Field)
#endif
                .Select(x => new { Attribute = x.GetCustomAttribute<DisplayNameAttribute>(), x.Name })
                .FirstOrDefault(x => x.Attribute != null && value.Equals(Enum.Parse(enumType, x.Name)))?.Attribute.DisplayName;
        }
    }
}