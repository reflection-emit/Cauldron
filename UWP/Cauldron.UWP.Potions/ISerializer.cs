using System;
using System.Threading.Tasks;

#if WINDOWS_UWP

using Windows.Storage;

#else

using System.IO;

#endif

namespace Cauldron.Potions
{
    /// <summary>
    /// Provides method that helps with serializing and deserializing an object
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="type">The object type to deserialize</param>
        /// <param name="name">The name of the file</param>
        /// <returns>An instance of the deserialized object</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="type"/> is a value type</exception>
        Task<object> DeserializeAsync(Type type, string name);

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
        Task<object> DeserializeAsync(Type type, StorageFolder folder, string name);

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
        Task<object> DeserializeAsync(Type type, DirectoryInfo folder, string name);

#endif

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
        Task<T> DeserializeAsync<T>(StorageFolder folder, string name) where T : class;

#else

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="folder">The directory where the file resides</param>
        /// <param name="name">The name of the file</param>
        /// <returns>An instance of the deserialized object</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="name"/> is a value type</exception>
        Task<T> DeserializeAsync<T>(DirectoryInfo folder, string name) where T : class;

#endif

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <returns>An instance of the deserialized object</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="name"/> is a value type</exception>
        Task<T> DeserializeAsync<T>(string name) where T : class;

#if WINDOWS_UWP

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="context">The object to serialize</param>
        /// <param name="name">The name of the file</param>
        /// <param name="folder">The directory where the file resides</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="context"/> is a value type</exception>
        void Serialize(object context, StorageFolder folder, string name);

#else

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="context">The object to serialize</param>
        /// <param name="name">The name of the file</param>
        /// <param name="folder">The directory where the file resides</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="context"/> is a value type</exception>
        void Serialize(object context, DirectoryInfo folder, string name);

#endif

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="context">The object to serialize</param>
        /// <param name="name">The name of the file</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="context"/> is a value type</exception>
        Task SerializeAsync(object context, string name);

#if WINDOWS_UWP

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="context">The object to serialize</param>
        /// <param name="name">The name of the file</param>
        /// <param name="folder">The directory where the file resides</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="context"/> is a value type</exception>
        Task SerializeAsync(object context, StorageFolder folder, string name);

#else

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="context">The object to serialize</param>
        /// <param name="name">The name of the file</param>
        /// <param name="folder">The directory where the file resides</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="context"/> is a value type</exception>
        Task SerializeAsync(object context, DirectoryInfo folder, string name);

#endif
    }
}