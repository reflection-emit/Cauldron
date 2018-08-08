using System;
using System.Text;
using System.ComponentModel;

#if WINDOWS_UWP

using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

#else

using System.Security.Cryptography;

#endif

namespace Cauldron.Interception
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static class ExtensionsInterception
    {
        /// <summary>
        /// Creates a new <see cref="Type"/> that implements the properties of an interface defined by <typeparamref name="T"/>
        /// and copies all value of <paramref name="anon"/> to the new object.
        /// </summary>
        /// <typeparam name="T">The type of interface to implement</typeparam>
        /// <param name="anon">The anonymous object</param>
        /// <returns>A new object implementing the interface defined by <typeparamref name="T"/></returns>
        /// <example>
        /// In some special cases (e.g. unit tests) it may be required to convert an anonymous type to a type that implements a certain interface.
        /// This also can be handy in situations where it is an overkill to create a new class.
        /// Lazy programmers will love this. To achieve this, a new type is weaved into your assembly.
        /// <para/>
        /// The following examples illustrates how the weaver modifies your code.
        /// <para/>
        /// Your code:
        /// <code>
        /// private static void Main(string[] args)
        /// {
        ///     var sample = new { Index = 0, Name = "Hello" }.CreateType&lt;ISampleInterface&gt;();
        ///     Console.WriteLine(sample.Name);
        /// }
        /// </code>
        /// Your code will look like this after the weaver's modification:
        /// <code>
        /// private static void Main(string[] args)
        /// {
        ///     var sample = Assign(new
        ///     {
        ///         Index = 0,
        ///         Name = "Hello"
        ///     });
        ///     Console.WriteLine(sample.Name);
        /// }
        /// </code>
        /// The weaver creates a new type that implements the ISampleInterface.
        /// <code>
        /// [EditorBrowsable(EditorBrowsableState.Never)]
        /// [Serializable]
        /// public sealed class SampleInterfaceCauldronAnonymousType : ISampleInterface
        /// {
        ///     public int Index { get; set; }
        ///     public string Name { get; set; }
        /// }
        /// </code>
        /// The weaver also adds a new method that maps the values from the anonymous type to the SampleInterfaceCauldronAnonymousType.
        /// <code>
        /// [EditorBrowsable(EditorBrowsableState.Never)]
        /// private static SampleInterfaceCauldronAnonymousType Assign(AnonymousType&lt;int, string&gt; anonymousType)
        /// {
        ///     return new SampleInterfaceCauldronAnonymousType
        ///     {
        ///         Index = anonymousType.Index,
        ///         Name = anonymousType.Name
        ///     };
        /// }
        /// </code>
        /// </example>
        public static T CreateType<T>(this object anon) where T : class
        {
            /* NOTE: This will be implemented by Cauldron.Interception.Fody */
            throw new NotImplementedException("No weaving happend.");
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string GetSHA256Hash(this string value)
        {
#if WINDOWS_UWP
            var sha = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha512);
            var buffer = CryptographicBuffer.ConvertStringToBinary(value, BinaryStringEncoding.Utf8);

            var hashed = sha.HashData(buffer);
            byte[] bytes;

            CryptographicBuffer.CopyToByteArray(hashed, out bytes);
            return Convert.ToBase64String(bytes);
#elif NETCORE

            using (var sha = SHA256.Create())
                return Convert.ToBase64String(sha.ComputeHash(UTF8Encoding.UTF8.GetBytes(value)));
#else
            using (var sha = SHA256.Create())
                return Convert.ToBase64String(sha.ComputeHash(UTF8Encoding.UTF8.GetBytes(value)));

#endif
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void TryDisposeInternal(this object context)
        {
            var disposable = context as IDisposable;
            disposable?.Dispose();
        }
    }
}