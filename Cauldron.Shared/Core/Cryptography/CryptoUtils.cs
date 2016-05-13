namespace Cauldron.Core.Cryptography
{
    /// <summary>
    /// Provides methods for cryptography
    /// </summary>
    [Factory(typeof(ICryptoUtils))]
    public sealed class CryptoUtils : Singleton<CryptoUtils>, ICryptoUtils
    {
        private const string PasswordCharactersSet = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890äöüÄÖÜ!§$%&/()=?'#+*~-_;,|<>³²^°@☺☼♀♂♠♣♥♦♪♫ﻇﺶﺭﺅﯼﯴ▬▐☺♀Ⱶ◄ℓ₥∑┤╟☺░⌠∆↨℗₰₯ḝւռ֎ϰ";

        /// <summary>
        /// Generates a random password
        /// </summary>
        /// <param name="length">The length of the password to generate</param>
        /// <returns>The generated password</returns>
        public string GeneratePassword(uint length)
        {
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
                result[i] = Randomizer.Current.Next<char>(PasswordCharactersSet.ToArray<char>());

            return new string(result);
        }
    }
}