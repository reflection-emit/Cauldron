using System;
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
        /// A character set of lower and upper case letters, numbers and special characters
        /// </summary>
        public const string AlphaNumericAndSpecialCharactersSet = AlphaNumericCharactersSet + @"?=()//&%$!;,:._-#'+*";

        /// <summary>
        /// A character set of lower and upper case letters and numbers
        /// </summary>
        public const string AlphaNumericCharactersSet = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

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
            var result = new char[length];

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

            if (password.Length < 4)
                return PasswordScore.VeryWeak;

            if (password.Length >= 8)
                score++;

            if (password.Length >= 12)
                score++;

            // If 90% of the password are numbers then lower the score
            if (MathEx.ValueOf(Regex.Match(password, @"[0-9]+(\.[0-9][0-9]?)?").Length, password.Length, 100) > 90)
                score--;
            // At least 4 of the chars should be numbers
            else if (Regex.Match(password, @"[0-9]+(\.[0-9][0-9]?)?").Length > 4)
                score++;

            // If 99% of the password are letters then lower the score
            if (MathEx.ValueOf(Regex.Match(password, @"^(?=.*[a-z])(?=.*[A-Z]).+$").Length, password.Length, 100) == 99)
                score--;
            else if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z]).+$"))
                score++;

            if (Regex.IsMatch(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]"))
                score++;

            if (string.Equals(password, "password", StringComparison.CurrentCultureIgnoreCase) ||
                string.Equals(password, "p4ssw0rd", StringComparison.CurrentCultureIgnoreCase) ||
                string.Equals(password, "p455w0rd", StringComparison.CurrentCultureIgnoreCase))
                score = 1;

            if (password.Distinct().Count() == 1)
                score = score - 2;

            return (PasswordScore)MathEx.Clamp(score, 1, 5);
        }
    }
}