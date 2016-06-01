namespace Cauldron.Core.Cryptography
{
    /// <summary>
    /// Represents methods for cryptography
    /// </summary>
    public interface ICryptoUtils
    {
        /// <summary>
        /// Generates a random password
        /// </summary>
        /// <param name="length">The length of the password to generate</param>
        /// <returns>The generated password</returns>
        string GeneratePassword(uint length);
    }
}