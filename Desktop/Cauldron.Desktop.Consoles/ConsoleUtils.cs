using Cauldron.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Consoles
{
    public static class ConsoleUtils
    {
        public static void WriteTable(params ConsoleTableColumn[] columns)
        {
            if (columns.Length == 0)
                return;

            // The maximum width each column can have
            var maxWidth = Console.WindowWidth / columns.Length;
            var remainingWidth = Console.WindowWidth - 1;

            foreach (var column in columns)
            {
                var width = column.Values.Max(x => x.Length) + 2 /* Add to chars space so that it does not look bad */;
                column.width = Math.Min(width, maxWidth);
                remainingWidth -= column.width;
            }

            // add the remaining width to the last column
            columns.Last().width += remainingWidth;

            var totalLine = columns.Max(x => x.Values.Count);

            foreach (var col in columns)
                col.text = new string[totalLine][];

            // set multiline and text wrap stuff
            for (int line = 0; line < totalLine; line++)
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    if (columns[i].Values.Count <= line)
                    {
                        columns[i].text[line] = new string[] { "" };
                        continue;
                    }

                    var text = columns[i].Values[line].GetLines();

                    if (!columns[i].WrapWords)
                        columns[i].text[line] = new string[] { text.Length == 0 ? "" : text[0] };
                    else
                    {
                        var additionalLines = new List<string>();

                        foreach (var p in text)
                        {
                            if (p.Length <= columns[i].width)
                                additionalLines.Add(p);
                            else
                                additionalLines.AddRange(p.WrapWords(columns[i].width - 10));
                        }

                        columns[i].text[line] = additionalLines.ToArray();
                    }
                }
            }

            foreach (var col in columns)
                col.Values.Clear();

            for (int line = 0; line < totalLine; line++)
            {
                // we have the maximum sub lines of all columns in the current line
                var maxlines = columns.Max(x => x.text[line].Length);

                foreach (var col in columns)
                    col.Values.AddRange(col.text[line].Pad(maxlines));
            }

            totalLine = columns.Max(x => x.Values.Count);

            for (int line = 0; line < totalLine; line++)
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    Console.ForegroundColor = columns[i].Foreground;
                    Console.BackgroundColor = columns[i].Background;
                    WriteAligned(columns[i].Values[line], columns[i].Alignment, columns[i].width, columns[i].Filler);
                }
                Console.Write("\n");
            }
        }

        private static string[] Pad(this string[] array, int totalItems)
        {
            if (array.Length >= totalItems)
                return array;

            var result = new string[Math.Max(totalItems, array.Length)];
            Array.Copy(array, result, array.Length);

            for (int i = array.Length; i < result.Length; i++)
                result[i] = string.Empty;

            return result;
        }

        private static string[] WrapWords(this string value, int maximumLength)
        {
            var result = new List<string>();
            var words = value.Split(' ');
            var currentLine = new List<string>();
            var totalLineLength = 0;

            foreach (var word in words)
            {
                if (totalLineLength + word.Length > maximumLength)
                {
                    result.Add(string.Join(" ", currentLine));
                    currentLine.Clear();
                    totalLineLength = 0;
                }

                currentLine.Add(word);
                totalLineLength += word.Length;
            }

            result.Add(string.Join(" ", currentLine));
            return result.ToArray();
        }

        private static void WriteAligned(string value, ConsoleTableColumnAlignment alignment, int widthLength, char filler)
        {
            string valueToWrite = "";

            switch (alignment)
            {
                case ConsoleTableColumnAlignment.Left:
                    if (value.Length > widthLength - 2)
                        valueToWrite = value.Substring(0, widthLength - 2).PadRight(widthLength, filler);
                    else
                        valueToWrite = value.PadRight(widthLength, filler);
                    break;

                case ConsoleTableColumnAlignment.Center:
                    var text = value.Length > widthLength - 2 ? value.Substring(value.Length - widthLength - 1, widthLength - 1) : value;
                    var centerText = text.Length / 2;
                    var centerColumn = widthLength / 2;
                    var pad = centerColumn - centerText;

                    valueToWrite = text.PadRight(widthLength - pad, filler).PadLeft(widthLength, filler);
                    break;

                case ConsoleTableColumnAlignment.Right:
                    if (value.Length > widthLength - 2)
                        valueToWrite = value.Substring(0, widthLength - 2).PadLeft(widthLength, filler);
                    else
                        valueToWrite = value.PadLeft(widthLength, filler);
                    break;
            }

            Console.Write(valueToWrite);
        }
    }
}