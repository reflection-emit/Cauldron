using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#if NETFX_CORE

using Windows.Storage;
using Windows.Storage.Streams;

#else

using System.IO;

#endif

namespace Cauldron.Core.Java
{
    internal static partial class Extensions
    {
        private static readonly Regex _getLinesRegex = new Regex("\r\n|\r|\n", RegexOptions.Compiled);

        public static string ConvertAnsiToUTF8(this string source)
        {
            var fromEncoding = Encoding.GetEncoding("ISO-8859-1");
            return Encoding.UTF8.GetString(Encoding.Convert(fromEncoding, Encoding.UTF8, fromEncoding.GetBytes(source)));
        }

        public static string ConvertUTF8ToAnsi(this string source)
        {
            var toEncoding = Encoding.GetEncoding("ISO-8859-1");
            return toEncoding.GetString(Encoding.Convert(Encoding.UTF8, toEncoding, Encoding.UTF8.GetBytes(source)));
        }

        public static string[] GetLines(this string value) => _getLinesRegex.Split(value);

        public static string ReadToEnd(this Stream stream)
        {
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }

    internal static partial class Extensions
    {
#if NETFX_CORE
        public static async Task<string> ReadTextAsync(this IStorageFile file)
            => await FileIO.ReadTextAsync(file);

        public static async Task WriteTextAsync(this IStorageFile file, string content)
            => await FileIO.WriteTextAsync(file, content);
#endif
    }

    internal static partial class Extensions
    {
#if !NETFX_CORE

        public static async Task<string> ReadTextAsync(this FileInfo file)
        {
            var result = default(string);
            await Task.Run(() => result = File.ReadAllText(file.FullName));
            return result;
        }

        public static async Task WriteTextAsync(this FileInfo file, string content)
                    => await Task.Run(() => File.WriteAllText(file.FullName, content, Encoding.Default));

#endif
    }
}