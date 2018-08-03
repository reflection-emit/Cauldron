using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Cauldron.Compression
{
    internal static class GZip
    {
        public static MemoryStream Unzip(Stream stream)
        {
            var target = new MemoryStream();

            using (var decompressionStream = new GZipStream(stream, CompressionMode.Decompress, true))
                decompressionStream.CopyTo(target);

            target.Seek(0, SeekOrigin.Begin);
            return target;
        }

        public static void Unzip(Stream stream, FileInfo target)
        {
            using (var file = target.OpenWrite())
            using (var decompressionStream = new GZipStream(stream, CompressionMode.Decompress, true))
                decompressionStream.CopyTo(file);
        }

        public static async Task UnzipAsync(Stream stream, FileInfo target)
        {
            using (var file = target.OpenWrite())
            using (var decompressionStream = new GZipStream(stream, CompressionMode.Decompress, true))
                await decompressionStream.CopyToAsync(file);
        }

        public static async Task<MemoryStream> UnzipAsync(Stream stream)
        {
            var target = new MemoryStream();

            using (var decompressionStream = new GZipStream(stream, CompressionMode.Decompress, true))
                await decompressionStream.CopyToAsync(target);

            target.Seek(0, SeekOrigin.Begin);
            return target;
        }

        public static MemoryStream Zip(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var target = new MemoryStream();

            using (var compressionStream = new GZipStream(target, CompressionLevel.Optimal, true))
                stream.CopyTo(compressionStream);

            target.Seek(0, SeekOrigin.Begin);
            return target;
        }

        public static void Zip(Stream stream, FileInfo targetFile)
        {
            stream.Seek(0, SeekOrigin.Begin);

            using (var target = targetFile.OpenWrite())
            using (var compressionStream = new GZipStream(target, CompressionLevel.Optimal, true))
                stream.CopyTo(compressionStream);
        }

        public static async Task ZipAsync(Stream stream, FileInfo targetFile)
        {
            stream.Seek(0, SeekOrigin.Begin);

            using (var target = targetFile.OpenWrite())
            using (var compressionStream = new GZipStream(target, CompressionLevel.Optimal, true))
                await stream.CopyToAsync(compressionStream);
        }

        public static async Task<MemoryStream> ZipAsync(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var target = new MemoryStream();

            using (var compressionStream = new GZipStream(target, CompressionLevel.Optimal, true))
                await stream.CopyToAsync(compressionStream);

            target.Seek(0, SeekOrigin.Begin);
            return target;
        }
    }
}