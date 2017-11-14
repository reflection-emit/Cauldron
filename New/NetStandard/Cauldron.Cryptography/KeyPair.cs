using Cauldron.Core;
using System;
using System.Security;

namespace Cauldron.Cryptography
{
    /// <summary>
    /// Represents the generated private and public key pair of the asymmethric algorithm
    /// </summary>
    public sealed class KeyPair : DisposableObject
    {
        internal KeyPair(SecureString privateKey, string publicKey)
        {
            this.PrivateKey = privateKey;
            this.PublicKey = publicKey;
        }

        /// <summary>
        /// Gets the private Key
        /// </summary>
        public SecureString PrivateKey { get; private set; }

        /// <summary>
        /// Gets the public key
        /// </summary>
        public string PublicKey { get; private set; }

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            if (disposeManaged)
                this.PrivateKey.Dispose();
        }
    }
}