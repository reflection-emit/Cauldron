using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel;

#if WINDOWS_UWP

using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;

#else

using System.Security.Cryptography;

#endif

namespace Cauldron.Core
{
    using Cauldron.Core.Diagnostics;

    /// <summary>
    /// Provides method that helps with serializing and deserializing an object
    /// </summary>
    public static class Serializer
    {
#if WINDOWS_UWP

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <typeparam name="T">The object type to deserialize</typeparam>
        /// <param name="name">The name of the file</param>
        /// <returns>An instance of the deserialized object</returns>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> is a value type</exception>
        public static async Task<T> DeserializeAsync<T>(string name) where T : class => await DeserializeAsync(typeof(T), name) as T;

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <typeparam name="T">The object type to deserialize</typeparam>
        /// <param name="folder">The directory where the file resides</param>
        /// <param name="name">The name of the file</param>
        /// <returns>An instance of the deserialized object</returns>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> is a value type</exception>
        public static async Task<T> DeserializeAsync<T>(StorageFolder folder, string name) where T : class =>
            await DeserializeAsync(typeof(T), ApplicationData.Current.LocalFolder, name) as T;

#else

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <typeparam name="T">The object type to deserialize</typeparam>
        /// <param name="name">The name of the file</param>
        /// <returns>An instance of the deserialized object</returns>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> is a value type</exception>
        public static async Task<T> DeserializeAsync<T>(string name) where T : class => await DeserializeAsync(typeof(T), name) as T;

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <typeparam name="T">The object type to deserialize</typeparam>
        /// <param name="folder">The directory where the file resides</param>
        /// <param name="name">The name of the file</param>
        /// <returns>An instance of the deserialized object</returns>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> is a value type</exception>
        public static async Task<T> DeserializeAsync<T>(DirectoryInfo folder, string name) where T : class =>
            await DeserializeAsync(typeof(T), ApplicationData.Current.LocalFolder, name) as T;

#endif

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="type">The object type to deserialize</param>
        /// <param name="name">The name of the file</param>
        /// <returns>An instance of the deserialized object</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="type"/> is a value type</exception>
        public static async Task<object> DeserializeAsync(Type type, string name) =>
          await DeserializeAsync(type, ApplicationData.Current.LocalFolder, name);

#if WINDOWS_UWP

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="type">The object type to deserialize</param>
        /// <param name="folder">The directory where the file resides</param>
        /// <param name="name">The name of the file</param>
        /// <returns>An instance of the deserialized object</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="type"/> is a value type</exception>
        public static async Task<object> DeserializeAsync(Type type, StorageFolder folder, string name)
#else

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="type">The object type to deserialize</param>
        /// <param name="folder">The directory where the file resides</param>
        /// <param name="name">The name of the file</param>
        /// <returns>An instance of the deserialized object</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="type"/> is a value type</exception>
        public static async Task<object> DeserializeAsync(Type type, DirectoryInfo folder, string name)

#endif
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.GetTypeInfo().IsValueType)
                throw new NotSupportedException($"Value types are not supported");

            try
            {
                var file = await folder.GetFileAsync($"{type.FullName.GetHash()}_{name}.json");

                if (file == null)
                    return type.GetDefaultInstance();

                var content = await file.ReadTextAsync();
                return JsonConvert.DeserializeObject(content, type);
            }
            catch (Exception e)
            {
                // This is one of those evil silent errors... But in this case we can definetely ignore this...
                // This should only used for saving application settings like window position and other non-critical stuff
                Debug.WriteLine(e.Message);
                return type.GetDefaultInstance();
            }
        }

#if NETSTANDARD2_0

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Serialize(object context, DirectoryInfo folder, string name)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var type = context.GetType();

            if (type.GetTypeInfo().IsValueType)
                throw new NotSupportedException($"Value Types are not supported");

            var result = JsonConvert.SerializeObject(context);
            var filename = Path.Combine(folder.FullName, $"{type.FullName.GetHash()}_{name}.json");

            if (File.Exists(filename))
                File.Delete(filename);

            File.WriteAllText(filename, result);
        }

#endif

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="context">The object to serialize</param>
        /// <param name="name">The name of the file</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="context"/> is a value type</exception>
        public static async Task SerializeAsync(object context, string name) =>
            await SerializeAsync(context, ApplicationData.Current.LocalFolder, name);

#if WINDOWS_UWP

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="context">The object to serialize</param>
        /// <param name="name">The name of the file</param>
        /// <param name="folder">The directory where the file resides</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="context"/> is a value type</exception>
        public static async Task SerializeAsync(object context, StorageFolder folder, string name)
#else

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="context">The object to serialize</param>
        /// <param name="name">The name of the file</param>
        /// <param name="folder">The directory where the file resides</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="context"/> is a value type</exception>
        public static async Task SerializeAsync(object context, DirectoryInfo folder, string name)
#endif
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var type = context.GetType();

            if (type.GetTypeInfo().IsValueType)
                throw new NotSupportedException($"Value Types are not supported");

            var result = JsonConvert.SerializeObject(context);
            var file = await folder.CreateFileAsync($"{type.FullName.GetHash()}_{name}.json", CreationCollisionOption.ReplaceExisting);
            await file.WriteTextAsync(result);
        }

        private static string GetHash(this string value)
        {
#if WINDOWS_UWP
            var sha = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var buffer = CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(value));

            var hashed = sha.HashData(buffer);
            byte[] bytes;

            CryptographicBuffer.CopyToByteArray(hashed, out bytes);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
#else
            using (var sha = SHA256.Create())
                return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(value)));

#endif
        }
    }
}