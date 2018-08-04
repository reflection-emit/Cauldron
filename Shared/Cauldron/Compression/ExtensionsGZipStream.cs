using Cauldron.Compression;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static partial class Extensions
    {
        #region Zip

        #region byte Array

        /// <summary>
        /// Compresses data using gzip
        /// </summary>
        /// <param name="data">The data to compress</param>
        /// <returns>A byte array of the compressed data.</returns>
        public static byte[] ZipAsBytes(this byte[] data)
        {
            using (var stream = data.ZipAsStream() as MemoryStream)
                return stream.ToArray();
        }

        /// <summary>
        /// Compresses data using gzip
        /// </summary>
        /// <param name="data">The data to compress</param>
        /// <returns>A byte array of the compressed data.</returns>
        public static async Task<byte[]> ZipAsBytesAsync(this byte[] data)
        {
            using (var stream = await data.ZipAsStreamAsync() as MemoryStream)
                return stream.ToArray();
        }

        /// <summary>
        /// Compresses data using gzip
        /// </summary>
        /// <param name="data">The data to compress</param>
        /// <param name="targetFile">The target file.</param>
        /// <param name="overwrite">Overwrite the file if exists.</param>
        /// <returns>The new compressed file represented by <see cref="FileInfo"/></returns>
        public static FileInfo ZipAsFile(this byte[] data, FileInfo targetFile, bool overwrite = false) => data.ZipAsFile(targetFile.FullName, overwrite);

        /// <summary>
        /// Compresses data using gzip
        /// </summary>
        /// <param name="data">The data to compress</param>
        /// <param name="path">The path and filename of the compressed file</param>
        /// <param name="overwrite">Overwrite the file if exists.</param>
        /// <returns>The new compressed file represented by <see cref="FileInfo"/></returns>
        public static FileInfo ZipAsFile(this byte[] data, string path, bool overwrite = false)
        {
            if (File.Exists(path) && !overwrite)
                throw new IOException("File already exists.");

            if (File.Exists(path) && overwrite)
                File.Delete(path);

            var target = new FileInfo(path);

            using (var dataStream = new MemoryStream(data))
                GZip.Zip(dataStream, target);

            return target;
        }

        /// <summary>
        /// Compresses data using gzip
        /// </summary>
        /// <param name="data">The data to compress</param>
        /// <param name="targetFile">The target file.</param>
        /// <param name="overwrite">Overwrite the file if exists.</param>
        /// <returns>The new compressed file represented by <see cref="FileInfo"/></returns>
        public static Task<FileInfo> ZipAsFileAsync(this byte[] data, FileInfo targetFile, bool overwrite = false) => data.ZipAsFileAsync(targetFile.FullName, overwrite);

        /// <summary>
        /// Compresses data using gzip
        /// </summary>
        /// <param name="data">The data to compress</param>
        /// <param name="path">The path and filename of the compressed file</param>
        /// <param name="overwrite">Overwrite the file if exists.</param>
        /// <returns>The new compressed file represented by <see cref="FileInfo"/></returns>
        public static async Task<FileInfo> ZipAsFileAsync(this byte[] data, string path, bool overwrite = false)
        {
            if (File.Exists(path) && !overwrite)
                throw new IOException("File already exists.");

            if (File.Exists(path) && overwrite)
                File.Delete(path);

            var target = new FileInfo(path);

            using (var dataStream = new MemoryStream(data))
                await GZip.ZipAsync(dataStream, target);

            return target;
        }

        /// <summary>
        /// Compresses data using gzip
        /// </summary>
        /// <param name="data">The data to compress</param>
        /// <returns>The compressed stream.</returns>
        public static Stream ZipAsStream(this byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return GZip.Zip(stream);
        }

        /// <summary>
        /// Compresses data using gzip
        /// </summary>
        /// <param name="data">The data to compress</param>
        /// <returns>The compressed stream.</returns>
        public static async Task<Stream> ZipAsStreamAsync(this byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return await GZip.ZipAsync(stream);
        }

        #endregion byte Array

        #region Stream

        /// <summary>
        /// Compresses a stream using gzip
        /// </summary>
        /// <param name="stream">The stream to compress</param>
        /// <returns>A byte array of the compressed data.</returns>
        public static byte[] ZipAsBytes(this Stream stream)
        {
            using (var data = stream.ZipAsStream() as MemoryStream)
                return data.ToArray();
        }

        /// <summary>
        /// Compresses a stream using gzip
        /// </summary>
        /// <param name="stream">The stream to compress</param>
        /// <returns>A byte array of the compressed data.</returns>
        public static async Task<byte[]> ZipAsBytesAsync(this Stream stream)
        {
            using (var data = await stream.ZipAsStreamAsync() as MemoryStream)
                return data.ToArray();
        }

        /// <summary>
        /// Comptesses a stream to a file using gzip
        /// </summary>
        /// <param name="stream">The stream to compress</param>
        /// <param name="file">The target file.</param>
        /// <param name="overwrite">Overwrite the file if exists.</param>
        /// <returns>The new compressed file represented by <see cref="FileInfo"/></returns>
        public static FileInfo ZipAsFile(this Stream stream, FileInfo file, bool overwrite = false) => stream.ZipAsFile(file.FullName, overwrite);

        /// <summary>
        /// Comptesses a stream to a file using gzip
        /// </summary>
        /// <param name="stream">The stream to compress</param>
        /// <param name="path">The path and filename of the compressed file</param>
        /// <param name="overwrite">Overwrite the file if exists.</param>
        /// <returns>The new compressed file represented by <see cref="FileInfo"/></returns>
        public static FileInfo ZipAsFile(this Stream stream, string path, bool overwrite = false)
        {
            if (File.Exists(path) && !overwrite)
                throw new IOException("File already exists.");

            if (File.Exists(path) && overwrite)
                File.Delete(path);

            var target = new FileInfo(path);
            stream.Seek(0, SeekOrigin.Begin);
            GZip.Zip(stream, target);

            return target;
        }

        /// <summary>
        /// Comptesses a stream to a file using gzip
        /// </summary>
        /// <param name="stream">The stream to compress</param>
        /// <param name="file">The target file.</param>
        /// <param name="overwrite">Overwrite the file if exists.</param>
        /// <returns>The new compressed file represented by <see cref="FileInfo"/></returns>
        public static Task<FileInfo> ZipAsFileAsync(this Stream stream, FileInfo file, bool overwrite = false) => stream.ZipAsFileAsync(file.FullName, overwrite);

        /// <summary>
        /// Comptesses a stream to a file using gzip
        /// </summary>
        /// <param name="stream">The stream to compress</param>
        /// <param name="path">The path and filename of the compressed file</param>
        /// <param name="overwrite">Overwrite the file if exists.</param>
        /// <returns>The new compressed file represented by <see cref="FileInfo"/></returns>
        public static async Task<FileInfo> ZipAsFileAsync(this Stream stream, string path, bool overwrite = false)
        {
            if (File.Exists(path) && !overwrite)
                throw new IOException("File already exists.");

            if (File.Exists(path) && overwrite)
                File.Delete(path);

            var target = new FileInfo(path);
            stream.Seek(0, SeekOrigin.Begin);
            await GZip.ZipAsync(stream, target);

            return target;
        }

        /// <summary>
        /// Compresses a stream using gzip
        /// </summary>
        /// <param name="stream">The stream to compress</param>
        /// <returns>The compressed stream.</returns>
        public static Stream ZipAsStream(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return GZip.Zip(stream);
        }

        /// <summary>
        /// Compresses a stream using gzip
        /// </summary>
        /// <param name="stream">The stream to compress</param>
        /// <returns>The compressed stream.</returns>
        public static async Task<Stream> ZipAsStreamAsync(this Stream stream)
        {
            var target = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);

            return await GZip.ZipAsync(stream);
        }

        #endregion Stream

        #region FileInfo

        /// <summary>
        /// Compresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to compress</param>
        /// <returns>A byte array of the compressed file.</returns>
        public static byte[] ZipAsBytes(this FileInfo file)
        {
            using (var stream = file.ZipAsStream() as MemoryStream)
                return stream.ToArray();
        }

        /// <summary>
        /// Compresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to compress</param>
        /// <returns>A byte array of the compressed file.</returns>
        public static async Task<byte[]> ZipAsBytesAsync(this FileInfo file)
        {
            using (var stream = await file.ZipAsStreamAsync() as MemoryStream)
                return stream.ToArray();
        }

        /// <summary>
        /// Compresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to compress</param>
        /// <param name="targetFile">The target file.</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The new compressed file represented by <see cref="FileInfo"/></returns>
        public static FileInfo ZipAsFile(this FileInfo file, FileInfo targetFile, bool overwrite = false) => file.ZipAsFile(targetFile.FullName, overwrite);

        /// <summary>
        /// Compresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to compress</param>
        /// <param name="path">The path and filename of the compressed file</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The new compressed file represented by <see cref="FileInfo"/></returns>
        public static FileInfo ZipAsFile(this FileInfo file, string path, bool overwrite = false)
        {
            if (File.Exists(path) && !overwrite)
                throw new IOException("File already exists.");

            if (File.Exists(path) && overwrite)
                File.Delete(path);

            var target = new FileInfo(path);

            using (var stream = file.OpenRead())
                GZip.Zip(stream, target);

            return target;
        }

        /// <summary>
        /// Compresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to compress</param>
        /// <param name="targetFile">The target file.</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The new compressed file represented by <see cref="FileInfo"/></returns>
        public static Task<FileInfo> ZipAsFileAsync(this FileInfo file, FileInfo targetFile, bool overwrite = false) => file.ZipAsFileAsync(targetFile.FullName, overwrite);

        /// <summary>
        /// Compresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to compress</param>
        /// <param name="path">The path and filename of the compressed file</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The new compressed file represented by <see cref="FileInfo"/></returns>
        public static async Task<FileInfo> ZipAsFileAsync(this FileInfo file, string path, bool overwrite = false)
        {
            if (File.Exists(path) && !overwrite)
                throw new IOException("File already exists.");

            if (File.Exists(path) && overwrite)
                File.Delete(path);

            var target = new FileInfo(path);

            using (var stream = file.OpenRead())
                await GZip.ZipAsync(stream, target);

            return target;
        }

        /// <summary>
        /// Compresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to compress</param>
        /// <returns>The compressed file as a stream.</returns>
        public static Stream ZipAsStream(this FileInfo file)
        {
            var target = new MemoryStream();

            using (var stream = file.OpenRead())
                return GZip.Zip(stream);
        }

        /// <summary>
        /// Compresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to compress</param>
        /// <returns>The compressed file as a stream.</returns>
        public static async Task<Stream> ZipAsStreamAsync(this FileInfo file)
        {
            var target = new MemoryStream();

            using (var stream = file.OpenRead())
                return await GZip.ZipAsync(stream);
        }

        #endregion FileInfo

        #region String

        /// <summary>
        /// Compresses a string using gzip. The default string encoding is <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <returns>A base 64 string representation of the compressed string.</returns>
        public static string ZipAsBase64String(this string value) => Convert.ToBase64String(value.ZipAsBytes());

        /// <summary>
        /// Compresses a string using gzip.
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns>A base 64 string representation of the compressed string.</returns>
        public static string ZipAsBase64String(this string value, Encoding encoding) => Convert.ToBase64String(value.ZipAsBytes(encoding));

        /// <summary>
        /// Compresses a string using gzip. The default string encoding is <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <returns>A base 64 string representation of the compressed string.</returns>
        public static async Task<string> ZipAsBase64StringAsync(this string value) => Convert.ToBase64String(await value.ZipAsBytesAsync());

        /// <summary>
        /// Compresses a string using gzip.
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns>A base 64 string representation of the compressed string.</returns>
        public static async Task<string> ZipAsBase64StringAsync(this string value, Encoding encoding) => Convert.ToBase64String(await value.ZipAsBytesAsync(encoding));

        /// <summary>
        /// Compresses a string using gzip. The default string encoding is <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <returns>A byte array of the compressed string.</returns>
        public static byte[] ZipAsBytes(this string value) => value.ZipAsBytes(Encoding.UTF8);

        /// <summary>
        /// Compresses a string using gzip
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns>A byte array of the compressed string.</returns>
        public static byte[] ZipAsBytes(this string value, Encoding encoding)
        {
            using (var stream = value.ZipAsStream(encoding) as MemoryStream)
                return stream.ToArray();
        }

        /// <summary>
        /// Compresses a string using gzip. The default string encoding is <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <returns>A byte array of the compressed string.</returns>
        public static Task<byte[]> ZipAsBytesAsync(this string value) => value.ZipAsBytesAsync(Encoding.UTF8);

        /// <summary>
        /// Compresses a string using gzip
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns>A byte array of the compressed string.</returns>
        public static async Task<byte[]> ZipAsBytesAsync(this string value, Encoding encoding)
        {
            using (var stream = await value.ZipAsStreamAsync(encoding) as MemoryStream)
                return stream.ToArray();
        }

        /// <summary>
        /// Compresses a string using gzip. The default string encoding is <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <returns>The compressed stream.</returns>
        public static Stream ZipAsStream(this string value) => value.ZipAsStream(Encoding.UTF8);

        /// <summary>
        /// Compresses a string using gzip
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns>The compressed stream.</returns>
        public static Stream ZipAsStream(this string value, Encoding encoding)
        {
            using (var source = new MemoryStream(encoding.GetBytes(value)))
                return GZip.Zip(source);
        }

        /// <summary>
        /// Compresses a string using gzip. The default string encoding is <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <returns>The compressed stream.</returns>
        public static Task<Stream> ZipAsStreamAsync(this string value) => value.ZipAsStreamAsync(Encoding.UTF8);

        /// <summary>
        /// Compresses a string using gzip
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns>The compressed stream.</returns>
        public static async Task<Stream> ZipAsStreamAsync(this string value, Encoding encoding)
        {
            var target = new MemoryStream();

            using (var source = new MemoryStream(encoding.GetBytes(value)))
                return await GZip.ZipAsync(source);
        }

        #endregion String

        #endregion Zip

        #region Unzip

        #region Stream

        /// <summary>
        /// Uncompresses a stream using gzip.
        /// </summary>
        /// <param name="stream">The stream to uncompress</param>
        /// <returns>The uncompressed stream as an array of bytes</returns>
        public static byte[] UnzipAsBytes(this Stream stream)
        {
            using (var result = stream.UnzipAsStream() as MemoryStream)
                return result.ToArray();
        }

        /// <summary>
        /// Uncompresses a stream using gzip.
        /// </summary>
        /// <param name="stream">The stream to uncompress</param>
        /// <returns>The uncompressed stream as an array of bytes</returns>
        public static async Task<byte[]> UnzipAsBytesAsync(this Stream stream)
        {
            using (var result = await stream.UnzipAsStreamAsync() as MemoryStream)
                return result.ToArray();
        }

        /// <summary>
        /// Uncompresses a stream using gzip.
        /// </summary>
        /// <param name="stream">The stream to uncompress</param>
        /// <param name="file">The file to write the uncompressed data.</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The <see cref="FileInfo"/> of the uncompressed data.</returns>
        public static FileInfo UnzipAsFile(this Stream stream, FileInfo file, bool overwrite = false) => stream.UnzipAsFile(file.FullName, overwrite);

        /// <summary>
        /// Uncompresses a stream using gzip.
        /// </summary>
        /// <param name="stream">The stream to uncompress</param>
        /// <param name="path">The path and filename of the file to write the uncompressed data.</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The <see cref="FileInfo"/> of the uncompressed data.</returns>
        public static FileInfo UnzipAsFile(this Stream stream, string path, bool overwrite = false)
        {
            if (File.Exists(path) && !overwrite)
                throw new IOException("File already exists.");

            if (File.Exists(path) && overwrite)
                File.Delete(path);

            var target = new FileInfo(path);

            GZip.Unzip(stream, target);

            return target;
        }

        /// <summary>
        /// Uncompresses a stream using gzip.
        /// </summary>
        /// <param name="stream">The stream to uncompress</param>
        /// <param name="file">The file to write the uncompressed data.</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The <see cref="FileInfo"/> of the uncompressed data.</returns>
        public static Task<FileInfo> UnzipAsFileAsync(this Stream stream, FileInfo file, bool overwrite = false) => stream.UnzipAsFileAsync(file.FullName, overwrite);

        /// <summary>
        /// Uncompresses a stream using gzip.
        /// </summary>
        /// <param name="stream">The stream to uncompress</param>
        /// <param name="path">The path and filename of the file to write the uncompressed data.</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The <see cref="FileInfo"/> of the uncompressed data.</returns>
        public static async Task<FileInfo> UnzipAsFileAsync(this Stream stream, string path, bool overwrite = false)
        {
            if (File.Exists(path) && !overwrite)
                throw new IOException("File already exists.");

            if (File.Exists(path) && overwrite)
                File.Delete(path);

            var target = new FileInfo(path);

            await GZip.UnzipAsync(stream, target);

            return target;
        }

        /// <summary>
        /// Uncompresses a stream using gzip
        /// </summary>
        /// <param name="stream">The stream to uncompress</param>
        /// <returns>The uncompressed stream.</returns>
        public static Stream UnzipAsStream(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return GZip.Unzip(stream);
        }

        /// <summary>
        /// Uncompresses a stream using gzip
        /// </summary>
        /// <param name="stream">The stream to uncompress</param>
        /// <returns>The uncompressed stream.</returns>
        public static async Task<Stream> UnzipAsStreamAsync(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return await GZip.UnzipAsync(stream);
        }

        /// <summary>
        /// Uncompresses a stream using gzip. The default encoding is <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="stream">The stream to uncompress</param>
        /// <returns>The uncompresses string.</returns>
        public static string UnzipAsString(this Stream stream) => stream.UnzipAsString(Encoding.UTF8);

        /// <summary>
        /// Uncompresses a stream using gzip.
        /// </summary>
        /// <param name="stream">The stream to uncompress</param>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns>The uncompresses string.</returns>
        public static string UnzipAsString(this Stream stream, Encoding encoding)
        {
            using (var output = GZip.Unzip(stream))
                return encoding.GetString(output.ToArray());
        }

        /// <summary>
        /// Uncompresses a stream using gzip. The default encoding is <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="stream">The stream to uncompress</param>
        /// <returns>The uncompresses string.</returns>
        public static Task<string> UnzipAsStringAsync(this Stream stream) => stream.UnzipAsStringAsync(Encoding.UTF8);

        /// <summary>
        /// Uncompresses a stream using gzip.
        /// </summary>
        /// <param name="stream">The stream to uncompress</param>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns>The uncompresses string.</returns>
        public static async Task<string> UnzipAsStringAsync(this Stream stream, Encoding encoding)
        {
            using (var output = await GZip.UnzipAsync(stream))
                return encoding.GetString(output.ToArray());
        }

        #endregion Stream

        #region FileInfo

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to uncompress</param>
        /// <returns>The uncompressed file as byte array</returns>
        public static byte[] UnzipAsBytes(this FileInfo file)
        {
            using (var stream = file.UnzipAsStream() as MemoryStream)
                return stream.ToArray();
        }

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to uncompress</param>
        /// <returns>The uncompressed file as byte array</returns>
        public static async Task<byte[]> UnzipAsBytesAsync(this FileInfo file)
        {
            using (var stream = await file.UnzipAsStreamAsync() as MemoryStream)
                return stream.ToArray();
        }

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to uncompress</param>
        /// <param name="targetFile">The file to write to the uncompressed data</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The file that contains the uncompressed data</returns>
        public static FileInfo UnzipAsFile(this FileInfo file, FileInfo targetFile, bool overwrite = false) => targetFile.UnzipAsFile(targetFile.FullName, overwrite);

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to uncompress</param>
        /// <param name="path">The path and filename of the file to write to the uncompressed data</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The file that contains the uncompressed data</returns>
        public static FileInfo UnzipAsFile(this FileInfo file, string path, bool overwrite = false)
        {
            if (File.Exists(path) && !overwrite)
                throw new IOException("File already exists.");

            if (File.Exists(path) && overwrite)
                File.Delete(path);

            var target = new FileInfo(path);

            using (var stream = file.OpenRead())
                GZip.Unzip(stream, target);

            return target;
        }

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to uncompress</param>
        /// <param name="targetFile">The file to write to the uncompressed data</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The file that contains the uncompressed data</returns>
        public static Task<FileInfo> UnzipAsFileAsync(this FileInfo file, FileInfo targetFile, bool overwrite = false) => targetFile.UnzipAsFileAsync(targetFile.FullName, overwrite);

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to uncompress</param>
        /// <param name="path">The path and filename of the file to write to the uncompressed data</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The file that contains the uncompressed data</returns>
        public static async Task<FileInfo> UnzipAsFileAsync(this FileInfo file, string path, bool overwrite = false)
        {
            if (File.Exists(path) && !overwrite)
                throw new IOException("File already exists.");

            if (File.Exists(path) && overwrite)
                File.Delete(path);

            var target = new FileInfo(path);

            using (var stream = file.OpenRead())
                await GZip.UnzipAsync(stream, target);

            return target;
        }

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to uncompress</param>
        /// <returns>The stream of uncompressed data.</returns>
        public static Stream UnzipAsStream(this FileInfo file)
        {
            using (var stream = file.OpenRead())
                return GZip.Unzip(stream);
        }

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="file">The file to uncompress</param>
        /// <returns>The stream of uncompressed data.</returns>
        public static async Task<Stream> UnzipAsStreamAsync(this FileInfo file)
        {
            using (var stream = file.OpenRead())
                return await GZip.UnzipAsync(stream);
        }

        #endregion FileInfo

        #region byte Array

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="bytes">The compressed data.</param>
        /// <returns>The uncompressed data as byte array.</returns>
        public static byte[] UnzipAsBytes(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
                return stream.UnzipAsBytes();
        }

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="bytes">The compressed data.</param>
        /// <returns>The uncompressed data as byte array.</returns>
        public static async Task<byte[]> UnzipAsBytesAsync(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
                return await stream.UnzipAsBytesAsync();
        }

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="bytes">The compressed data.</param>
        /// <param name="file">The file to write to the uncompressed data</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The file that contains the uncompressed data</returns>
        public static FileInfo UnzipAsFile(this byte[] bytes, FileInfo file, bool overwrite = false)
        {
            using (var stream = new MemoryStream(bytes))
                return stream.UnzipAsFile(file, overwrite);
        }

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="bytes">The compressed data.</param>
        /// <param name="path">The path and filename of the file to write to the uncompressed data</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The file that contains the uncompressed data</returns>
        public static FileInfo UnzipAsFile(this byte[] bytes, string path, bool overwrite = false)
        {
            using (var stream = new MemoryStream(bytes))
                return stream.UnzipAsFile(path, overwrite);
        }

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="bytes">The compressed data.</param>
        /// <param name="file">The file to write to the uncompressed data</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The file that contains the uncompressed data</returns>
        public static async Task<FileInfo> UnzipAsFileAsync(this byte[] bytes, FileInfo file, bool overwrite = false)
        {
            using (var stream = new MemoryStream(bytes))
                return await stream.UnzipAsFileAsync(file, overwrite);
        }

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="bytes">The compressed data.</param>
        /// <param name="path">The path and filename of the file to write to the uncompressed data</param>
        /// <param name="overwrite">If true, overwrites the file if exists; otherwise false</param>
        /// <returns>The file that contains the uncompressed data</returns>
        public static async Task<FileInfo> UnzipAsFileAsync(this byte[] bytes, string path, bool overwrite = false)
        {
            using (var stream = new MemoryStream(bytes))
                return await stream.UnzipAsFileAsync(path, overwrite);
        }

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="bytes">The compressed data.</param>
        /// <returns>The stream that contains the uncompressed data.</returns>
        public static Stream UnzipAsStream(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
                return stream.UnzipAsStream();
        }

        /// <summary>
        /// Uncompresses a file using gzip.
        /// </summary>
        /// <param name="bytes">The compressed data.</param>
        /// <returns>The stream that contains the uncompressed data.</returns>
        public static async Task<Stream> UnzipAsStreamAsync(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
                return await stream.UnzipAsStreamAsync();
        }

        /// <summary>
        /// Uncompresses data using gzip. The default encoding is <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="bytes">The compressed data.</param>
        /// <returns>The uncompressed string</returns>
        public static string UnzipAsString(this byte[] bytes) => bytes.UnzipAsString(Encoding.UTF8);

        /// <summary>
        /// Uncompresses data using gzip
        /// </summary>
        /// <param name="bytes">The compressed data.</param>
        /// <param name="encoding">The encoding of the compressed string</param>
        /// <returns>The uncompressed string</returns>
        public static string UnzipAsString(this byte[] bytes, Encoding encoding)
        {
            using (var stream = new MemoryStream(bytes))
                return encoding.GetString(stream.UnzipAsBytes());
        }

        /// <summary>
        /// Uncompresses data using gzip. The default encoding is <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="bytes">The compressed data.</param>
        /// <returns>The uncompressed string</returns>
        public static Task<string> UnzipAsStringAsync(this byte[] bytes) => bytes.UnzipAsStringAsync(Encoding.UTF8);

        /// <summary>
        /// Uncompresses data using gzip
        /// </summary>
        /// <param name="bytes">The compressed data.</param>
        /// <param name="encoding">The encoding of the compressed string</param>
        /// <returns>The uncompressed string</returns>
        public static async Task<string> UnzipAsStringAsync(this byte[] bytes, Encoding encoding)
        {
            using (var stream = new MemoryStream(bytes))
                return encoding.GetString(await stream.UnzipAsBytesAsync());
        }

        #endregion byte Array

        #region String

        /// <summary>
        /// Uncompresses compressed string represented as base 64 string using gzip
        /// </summary>
        /// <param name="value">The base 64 string</param>
        /// <param name="encoding">The encoding of the compressed string</param>
        /// <returns>The uncompressed string</returns>
        public static string UnzipAsString(this string value, Encoding encoding)
        {
            if (!value.IsBase64String())
                throw new ArgumentException("value must be a Base64 string");

            using (var data = new MemoryStream(Convert.FromBase64String(value)))
                return data.UnzipAsString(encoding);
        }

        /// <summary>
        /// Uncompresses compressed string represented as base 64 string using gzip
        /// </summary>
        /// <param name="value">The base 64 string</param>
        /// <param name="encoding">The encoding of the compressed string</param>
        /// <returns>The uncompressed string</returns>
        public static async Task<string> UnzipAsStringAsync(this string value, Encoding encoding)
        {
            if (!value.IsBase64String())
                throw new ArgumentException("value must be a Base64 string");

            using (var data = new MemoryStream(Convert.FromBase64String(value)))
                return await data.UnzipAsStringAsync(encoding);
        }

        #endregion String

        #endregion Unzip
    }
}