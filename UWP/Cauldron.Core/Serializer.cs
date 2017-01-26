using Cauldron.Core.Extensions;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

#if ANDROID

using AndroidXml = System.Xml.Serialization;

#endif

namespace Cauldron.Core
{
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
                var file = await folder.GetFileAsync($"{type.FullName.GetHash()}_{name}.xml");

                if (file == null)
                    return type.GetDefaultInstance();
#if WINDOWS_UWP

                using (var stream = await file.OpenSequentialReadAsync())
                {
                    var serializer = new DataContractSerializer(type);
                    return serializer.ReadObject(stream.AsStreamForRead());
                }
#else
                using (var stream = file.OpenRead())
                {
#if ANDROID
                    var serializer = new AndroidXml.XmlSerializer(type);
                    return serializer.Deserialize(stream);
#else
                    var serializer = new DataContractSerializer(type);
                    return serializer.ReadObject(stream);
#endif
                }
#endif
            }
            catch (Exception e)
            {
                Output.WriteLineError(e.Message);
                return type.GetDefaultInstance();
            }
        }

#if WINDOWS_UWP

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="context">The object to serialize</param>
        /// <param name="name">The name of the file</param>
        /// <param name="folder">The directory where the file resides</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="context"/> is a value type</exception>
        public static void Serialize(object context, StorageFolder folder, string name)
#else

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="context">The object to serialize</param>
        /// <param name="name">The name of the file</param>
        /// <param name="folder">The directory where the file resides</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is null</exception>
        /// <exception cref="NotSupportedException"><paramref name="context"/> is a value type</exception>
        public static void Serialize(object context, DirectoryInfo folder, string name)
#endif
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var type = context.GetType();

            if (type.GetTypeInfo().IsValueType)
                throw new NotSupportedException($"Value Types are not supported");

            var ms = new MemoryStream();

#if ANDROID
            var serializer = new AndroidXml.XmlSerializer(type);
            serializer.Serialize(ms, context);
#else
            var serializer = new DataContractSerializer(type);
            serializer.WriteObject(ms, context);
#endif

#if WINDOWS_UWP
            var func = new Func<Task>(async () =>
            {
                var file = await folder.CreateFileAsync($"{type.FullName.GetHash()}_{name}.xml", CreationCollisionOption.ReplaceExisting).AsTask();

                using (var fs = await file.OpenStreamForWriteAsync())
                {
                    ms.Seek(0, SeekOrigin.Begin);

                    await ms.CopyToAsync(fs);
                    await fs.FlushAsync();
                }
            });
            func().RunSync();
#else
            var file = folder.CreateFileAsync($"{type.FullName.GetHash()}_{name}.xml", CreationCollisionOption.ReplaceExisting).RunSync();

            using (var fs = file.OpenWrite())
            {
                ms.Seek(0, SeekOrigin.Begin);

                ms.CopyTo(fs);
                fs.Flush();
            }

#endif
        }

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

            var ms = new MemoryStream();

#if ANDROID
            var serializer = new AndroidXml.XmlSerializer(type);
            serializer.Serialize(ms, context);
#else
            var serializer = new DataContractSerializer(type);
            serializer.WriteObject(ms, context);
#endif

            var file = await folder.CreateFileAsync($"{type.FullName.GetHash()}_{name}.xml", CreationCollisionOption.ReplaceExisting);
#if WINDOWS_UWP

            using (var fs = await file.OpenStreamForWriteAsync())
#else
            using (var fs = file.OpenWrite())
#endif
            {
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(fs);
                await fs.FlushAsync();
            }
        }
    }
}