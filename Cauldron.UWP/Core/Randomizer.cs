using System;
using Windows.Security.Cryptography;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides a randomizer that is cryptographicly secure
    /// </summary>
    public partial class Randomizer
    {
        private static int GetCryptographicSeed()
        {
            var random = new Random((int)CryptographicBuffer.GenerateRandomNumber());
            return random.Next();
        }
    }
}