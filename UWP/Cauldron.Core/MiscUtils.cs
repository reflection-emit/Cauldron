using Cauldron.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides static methods
    /// </summary>
    public static class MiscUtils
    {
        /// <summary>
        /// Generates a random lorem ipsum text
        /// </summary>
        /// <param name="minWords">The minimum word count to generate</param>
        /// <param name="maxWords">The maximum word count to generate</param>
        /// <param name="minSentences">The minimum sentence count to generate</param>
        /// <param name="maxSentences">The maximum sentence count to generate</param>
        /// <param name="paragraphCount">The number of paragraphs to generate</param>
        /// <returns>The generated lorem ipsum text</returns>
        /// <exception cref="ArgumentException"><paramref name="minWords"/> is 0</exception>
        /// <exception cref="ArgumentException"><paramref name="minSentences"/> is 0</exception>
        /// <exception cref="ArgumentException"><paramref name="paragraphCount"/> is 0</exception>
        /// <exception cref="ArgumentException"><paramref name="minWords"/> is greater than <paramref name="maxWords"/></exception>
        /// <exception cref="ArgumentException"><paramref name="minSentences"/> is greater than <paramref name="maxSentences"/></exception>
        public static string GenerateLoremIpsum(int minWords, int maxWords, int minSentences = 1, int maxSentences = 1, uint paragraphCount = 1)
        {
            if (paragraphCount == 0)
                throw new ArgumentException("Parameter 'paragraphCount' cannot be 0");

            if (minWords == 0)
                throw new ArgumentException("Parameter 'minWords' cannot be 0");

            if (minSentences == 0)
                throw new ArgumentException("Parameter 'minSentences' cannot be 0");

            if (minWords > maxWords)
                throw new ArgumentException("'minWords' cannot be greater than 'maxWords'");

            if (minSentences > maxSentences)
                throw new ArgumentException("'minSentences' cannot be greater than 'maxSentences'");

            var words = new[]
            {
                "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
                "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
                "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"
            };

            int numSentences = Randomizer.Next(minSentences, maxSentences);
            int numWords = Randomizer.Next(minWords, maxWords);
            var result = new string[paragraphCount];
            var creator = new Func<string>(() =>
            {
                var sb = new StringBuilder();

                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0)
                            sb.Append(" ");
                        sb.Append(Randomizer.Next(words));
                    }
                    sb.Append(". ");
                }

                return sb.ToString();
            });

            for (int i = 0; i < paragraphCount; i++)
                result[i] = creator();

            return result.Join("\r\n").TrimEnd();
        }

        /// <summary>
        /// Returns the <see cref="DisplayNameAttribute.DisplayName"/> of an enum type.
        /// </summary>
        /// <typeparam name="TEnum">The enum type</typeparam>
        /// <returns>A dictionary of display names with the enum value member as key</returns>
        public static IReadOnlyDictionary<TEnum, string> GetDisplayNames<TEnum>() where TEnum : struct, IConvertible
        {
            var enumType = typeof(TEnum);

            if (!enumType.GetTypeInfo().IsEnum)
                throw new ArgumentException($"{enumType.FullName} is not an enum type");

            return enumType
                .GetMembers()
#if !WINDOWS_UWP
                .Where(x => x.MemberType == MemberTypes.Field)
#endif
                .Select(x => new { Attribute = x.GetCustomAttribute<DisplayNameAttribute>(), Name = x.Name })
                .Where(x => x.Attribute != null)
                .ToDictionary(x => (TEnum)Enum.Parse(enumType, x.Name), x => x.Attribute.DisplayName)
                .AsReadOnly();
        }
    }
}