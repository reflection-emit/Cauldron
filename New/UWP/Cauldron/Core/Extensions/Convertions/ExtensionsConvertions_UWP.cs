using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Cauldron
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static partial class ExtensionsConvertions
    {
        /// <summary>
        /// Converts a <see cref="IRandomAccessStream"/> to <see cref="byte"/> array
        /// </summary>
        /// <param name="stream">The <see cref="IRandomAccessStream"/> to convert</param>
        /// <returns>An array of bytes</returns>
        public static async Task<byte[]> ToBytesAsync(this IRandomAccessStream stream)
        {
            byte[] buffer = new byte[stream.Size];

            using (DataReader reader = new DataReader(stream))
            {
                await reader.LoadAsync((uint)stream.Size);
                reader.ReadBytes(buffer);
                reader.DetachStream();
            }

            return buffer;
        }

        /// <summary>
        /// Converts a <see cref="Stream"/> to a <see cref="IRandomAccessStream"/>
        /// </summary>
        /// <param name="stream">The stream to convert</param>
        /// <returns>A new instance of <see cref="IRandomAccessStream"/></returns>
        public static async Task<IRandomAccessStream> ToRandomAccessStreamAsync(this Stream stream)
        {
            if (stream == null)
                return null;

            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);

            var result = new InMemoryRandomAccessStream();

            var dataWriter = new DataWriter(result);
            dataWriter.WriteBytes(buffer);

            await dataWriter.StoreAsync();

            result.Seek(0);

            return result;
        }
    }
}