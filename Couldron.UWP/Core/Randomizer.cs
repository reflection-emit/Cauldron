using System;
using Windows.Security.Cryptography;

namespace Couldron.Core
{
    /// <summary>
    /// Provides a randomizer that is cryptographicly secure
    /// </summary>
    public static partial class Randomizer
    {
        private static Random _global = new Random(GetCryptographicSeed());

        private static int GetCryptographicSeed()
        {
            return (int)CryptographicBuffer.GenerateRandomNumber();
        }
    }
}