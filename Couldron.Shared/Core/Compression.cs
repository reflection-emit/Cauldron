using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides methods for compressing and decompressing data
    /// </summary>
    public static class Compression
    {
        /// <summary>
        /// Compresses the data with gzip
        /// </summary>
        /// <param name="bytes">The bytes to compress</param>
        /// <returns>Compressed data</returns>
        public static byte[] Compress(byte[] bytes)
        {
            if (bytes == null || (bytes != null && bytes.Length == 0))
                return null;

            using (var msi = new MemoryStream(bytes))
            {
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                        CopyTo(msi, gs);
                    }

                    return mso.ToArray();
                }
            }
        }

        /// <summary>
        /// Compresses a string (UTF8) with gzip
        /// </summary>
        /// <param name="text">The string to compress</param>
        /// <returns>Compressed data</returns>
        public static byte[] CompressFromString(string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            return Compress(data);
        }

        /// <summary>
        /// Compresses data to base 64 string
        /// </summary>
        /// <param name="data">The data to compress</param>
        /// <returns>The compressed data represented by base 64 string</returns>
        public static string CompressToBase64String(byte[] data)
        {
            byte[] zipped = Compress(data);
            return Convert.ToBase64String(zipped, 0, zipped.Length);
        }

        /// <summary>
        /// Compresses a string (UTF8) to base 64 string
        /// </summary>
        /// <param name="text">The string to compress</param>
        /// <returns>The compressed string represented by base 64 srting</returns>
        public static string CompressToBase64String(string text)
        {
            var data = CompressFromString(text);
            return Convert.ToBase64String(data, 0, data.Length);
        }

        /// <summary>
        /// Decompresses data that was compressed with gzip
        /// </summary>
        /// <param name="bytes">The compressed data</param>
        /// <returns>The decompressed data</returns>
        public static byte[] Decompress(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            {
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        CopyTo(gs, mso);
                    }

                    return mso.ToArray();
                }
            }
        }

        /// <summary>
        /// Decompresses compressed data represented by base 64 string to bytes
        /// </summary>
        /// <param name="text">The bas 64 string to decompress</param>
        /// <returns>The decompressed data</returns>
        public static byte[] DecompressFromBase64String(string text)
        {
            return Decompress(Convert.FromBase64String(text));
        }

        /// <summary>
        /// Decompresses compressed data represented by base 64 string to a string
        /// </summary>
        /// <param name="text">The bas 64 string to decompress</param>
        /// <returns>The decompressed string</returns>
        public static string DecompressFromBase64StringToString(string text)
        {
            var data = Decompress(Convert.FromBase64String(text));
            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        /// <summary>
        /// Decompresses compressed data to a string (UTF8)
        /// </summary>
        /// <param name="bytes">The compressed data</param>
        /// <returns>The decompressed string (UTF8)</returns>
        public static string DecompressToString(byte[] bytes)
        {
            if (bytes == null || (bytes != null && bytes.Length == 0))
                return string.Empty;

            var data = Decompress(bytes);
            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
                dest.Write(bytes, 0, cnt);
        }
    }
}