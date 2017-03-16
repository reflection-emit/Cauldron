using Cauldron.Core.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron.Core.Java
{
    /// <summary>
    /// Reads and writes a java property file
    /// </summary>
    public sealed class JavaProperties : KeyedCollection<string, PropertyLine>
    {
        /// <summary>
        /// Parses the java property file from a string
        /// </summary>
        /// <param name="body">The content of the property file</param>
        /// <returns>A new instance of <see cref="JavaProperties"/></returns>
        public static JavaProperties Read(string body)
        {
            var sb = new StringBuilder();

            // concat lines that are ending with \\
            var lines = body.GetLines();
            for (int i = 0; i < lines.Length; i++)
            {
                var trimedLine = lines[i].Trim();

                if (trimedLine == "")
                    continue;

                sb.Append(trimedLine);

                if (!trimedLine.EndsWith("\\") || lines[i].EndsWith(" "))
                    sb.Append("\r\n");
                else if (sb[sb.Length - 1] == '\\')
                    sb[sb.Length - 1] = ' ';
            }

            var result = new JavaProperties();

            foreach (var line in sb.ToString().GetLines())
            {
                if (line == null || line.StartsWith("#") || line.StartsWith("!") || line == "")
                    continue;

                result.Add(new PropertyLine(line));
            }

            return result;
        }

        /// <summary>
        /// Parses the java property file from a stream
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <returns>A new instance of <see cref="JavaProperties"/></returns>
        public static JavaProperties Read(Stream stream) => Read(stream.ReadToEnd().Convert(Encodings.ANSI, Encodings.UTF8));

#if WINDOWS_UWP

        /// <summary>
        /// Parses the java property file from a file
        /// </summary>
        /// <param name="file">The property file</param>
        /// <returns>A new instance of <see cref="JavaProperties"/></returns>
        public static async Task<JavaProperties> ReadAsync(StorageFile file) => Read((await file.ReadTextAsync()).Convert(Encodings.ANSI, Encodings.UTF8));

        /// <summary>
        /// Saves the <see cref="JavaProperties"/> instance to a file
        /// </summary>
        /// <param name="file">The property file</param>
        /// <returns>An awaitable task</returns>
        public async Task SaveAsync(StorageFile file) => await file.WriteTextAsync(this.ToString().Convert(Encodings.UTF8, Encodings.ANSI));

#else

        /// <summary>
        /// Parses the java property file from a file
        /// </summary>
        /// <param name="file">The property file</param>
        /// <returns>A new instance of <see cref="JavaProperties"/></returns>
        public static async Task<JavaProperties> ReadAsync(FileInfo file) => Read((await file.ReadTextAsync()).Convert(Encodings.ANSI, Encodings.UTF8));

        /// <summary>
        /// Saves the <see cref="JavaProperties"/> instance to a file
        /// </summary>
        /// <param name="file">The property file</param>
        /// <returns>An awaitable task</returns>
        public async Task SaveAsync(FileInfo file) => await file.WriteTextAsync(this.ToString().Convert(Encodings.UTF8, Encodings.ANSI));

#endif

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("# " + DateTime.Now.ToString());

            foreach (var item in this)
                sb.AppendLine(item.ToString());

            return sb.ToString().Replace("\t", "\\t").Replace("\f", "\\f");
        }

        /// <summary>
        /// When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// <param name="item">The element from which to extract the key.</param>
        /// <returns>The key for the specified element.</returns>
        protected override string GetKeyForItem(PropertyLine item) => item.Key;
    }

    /// <summary>
    /// Represents a line in the <see cref="JavaProperties"/>
    /// </summary>
    [DebuggerDisplay("{Key} = {Value}")]
    public sealed class PropertyLine
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PropertyLine"/>
        /// </summary>
        /// <param name="key">The key of property</param>
        /// <param name="value">The value of the property</param>
        public PropertyLine(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        internal PropertyLine(string line)
        {
            var buffer = new char[line.Length];
            var escaped = false;
            var index = 0;

            for (int i = 0; i < line.Length; i++)
            {
                if ((line[i] == ' ' && !escaped) || line[i] == ':' || line[i] == '=')
                {
                    this.Key = this.Parse(new string(buffer, 0, index));

                    if (line.Length > i + 2 && (line[i + 1] == ':' || line[i + 1] == '='))
                        this.Value = this.Parse(line.Substring(i + 2).TrimStart());
                    else if (line.Length > i + 1 && (line[i + 1] == ':' || line[i + 1] == '='))
                        this.Value = "";
                    else if (line.Length > i + 1)
                        this.Value = this.Parse(line.Substring(i + 1).TrimStart());

                    break;
                }

                buffer[index++] = line[i];
                escaped = line[i] == '\\';
            }
        }

        /// <summary>
        /// Gets the key of the value
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets or sets a value of ta value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Value))
                return this.Replace(this.Key, true);

            return this.Replace(this.Key, true) + "=" + this.Replace(this.Value, false);
        }

        private string Parse(string str)
        {
            var buffer = new char[str.Length];
            var escaped = false;
            var index = 0;

            for (int i = 0; i < str.Length; i++)
            {
                if (escaped)
                {
                    switch (str[i])
                    {
                        case 't':
                            buffer[index++] = '\t';
                            break;

                        case 'f':
                            buffer[index++] = '\f';
                            break;

                        case 'u':

                            buffer[index++] = this.ToUnicode(str.Substring(i + 1, 4));
                            i += 4;
                            break;

                        default:
                            buffer[index++] = str[i];
                            break;
                    }

                    escaped = false;
                    continue;
                }

                if (str[i] == '\\')
                    escaped = true;
                else
                    buffer[index++] = str[i];
            }

            return new string(buffer, 0, index);
        }

        private string Replace(string str, bool isKey)
        {
            if (str == null || str.Length == 0)
                return str;

            var sb = new StringBuilder();

            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case '\t':
                        sb.Append("\\t");
                        break;

                    case '\f':
                        sb.Append("\\f");
                        break;

                    case ' ':
                        if (isKey)
                            sb.Append("\\ ");
                        else
                            sb.Append(" ");
                        break;

                    default:

                        if (str[i] > 255)
                        {
                            var hexString = ((int)str[i]).ToString("X2");
                            sb.Append("\\u" + hexString);
                        }
                        else
                            sb.Append(str[i]);
                        break;
                }
            }

            return sb.ToString();
        }

        private char ToUnicode(string unicodeHexstring) => (char)int.Parse(unicodeHexstring, System.Globalization.NumberStyles.HexNumber);
    }
}