namespace Couldron.Core.Cryptography
{
    /// <summary>
    /// Provides methods for cryptography
    /// </summary>
    public static class CryptoUtils
    {
        private const string PasswordCharactersSet = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890äöüÄÖÜ!§$%&/()=?'#+*~-_;,|<>³²^°@☺☼♀♂♠♣♥♦♪♫ﻇﺶﺭﺅﯼﯴ▬▐☺♀Ⱶ◄ℓ₥∑┤╟☺░⌠∆↨℗₰₯ḝւռ֎ϰ";

        /// <summary>
        /// Generates a random password
        /// </summary>
        /// <param name="length">The length of the password to generate</param>
        /// <returns>The generated password</returns>
        public static string GeneratePassword(uint length)
        {
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
                result[i] = Randomizer.Next<char>(PasswordCharactersSet.ToArray<char>());

            return new string(result);
        }
    }
}