using Cauldron.Core;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cauldron.Cryptography
{
    /// <summary>
    /// Provides methods for cryptography
    /// </summary>
    public static class CryptoUtils
    {
        /// <summary>
        /// A character set of lower and upper case letters and numbers
        /// </summary>
        public const string AlphaNumericCharactersSet = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890?=()//&%$!;,:._-#'+*";

        /// <summary>
        /// Generates a random password
        /// </summary>
        /// <param name="length">The length of the password to generate</param>
        /// <returns>The generated password</returns>
        public static string BrewPassword(uint length) =>
            BrewPassword(AlphaNumericCharactersSet, length);

        /// <summary>
        /// Generates a random password
        /// </summary>
        /// <param name="characterSet">The set of characters to generate the password from</param>
        /// <param name="length">The length of the password to generate</param>
        /// <returns>The generated password</returns>
        public static string BrewPassword(string characterSet, uint length)
        {
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
                result[i] = Randomizer.Next<char>(characterSet.ToArray<char>());

            return new string(result);
        }

        /// <summary>
        /// Checks the password's strength
        /// </summary>
        /// <param name="password">The password to check</param>
        /// <returns>Returns <see cref="PasswordScore"/> rating</returns>
        public static PasswordScore GetPasswordScore(string password)
        {
            // Origin: http://social.msdn.microsoft.com/Forums/vstudio/en-US/5e3f27d2-49af-410a-85a2-3c47e3f77fb1/how-to-check-for-password-strength
            if (string.IsNullOrEmpty(password))
                return PasswordScore.Blank;

            int score = 1;

            if (password.Length < 1)
                return PasswordScore.Blank;

            if (password.Length < 4)
                return PasswordScore.VeryWeak;

            if (password.Length >= 8)
                score++;

            if (password.Length >= 12)
                score++;

            // number only //"^\d+$" if you need to match more than one digit.
            if (Regex.IsMatch(password, @"[0-9]+(\.[0-9][0-9]?)?"))
                score++;

            // both, lower and upper case
            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z]).+$"))
                score++;

            // ^[A-Z]+$
            if (Regex.IsMatch(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]"))
                score++;

            return (PasswordScore)score;
        }
    }
}