using System.Security;
using System.Security.Cryptography;

namespace Cauldron.Core.Cryptography
{
    /// <summary>
    /// Provides methods to create a key material
    /// </summary>
    public sealed class KeyMaterial
    {
        private byte[] initialisationVector;
        private byte[] key;
        private byte[] salt;

        private KeyMaterial(SecureString password, byte[] salt, int iterations)
        {
            Rfc2898DeriveBytes keyMaterial;

            using (var handler = new SecureStringHandler(password))
            {
                keyMaterial = new Rfc2898DeriveBytes(handler.ToBytes(), salt, iterations);
            }

            this.salt = Compression.Compress(salt);
            this.initialisationVector = Compression.Compress(keyMaterial.GetBytes(16));
            this.key = Compression.Compress(keyMaterial.GetBytes(32));
        }

        public byte[] InitialisationVector
        {
            get { return Compression.Decompress(this.initialisationVector); }
        }

        public byte[] Key
        {
            get { return Compression.Decompress(this.key); }
        }

        public byte[] Salt
        {
            get { return Compression.Decompress(this.salt); }
        }

        public static KeyMaterial CreateKeyMaterial(SecureString password, byte[] salt, int iterations = 60000)
        {
            return new KeyMaterial(password, salt, iterations);
        }

        public static KeyMaterial CreateKeyMaterial(SecureString password, int iterations = 60000)
        {
            return new KeyMaterial(password, CryptoUtils.GeneratePassword(42).GetSha256HashBytes().GetBytes(16), iterations);
        }
    }
}