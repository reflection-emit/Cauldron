using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Consoles
{
    /// <summary>
    /// A collection of console utilities
    /// </summary>
    public static class ConsoleUtils
    {
#if !NETCORE

        /// <summary>
        /// Retrieves the window handle used by the console associated with the calling process.
        /// </summary>
        /// <returns>The return value is a handle to the window used by the console associated with the calling process or NULL if there is no such window associated console.</returns>
        public static IntPtr GetConsoleWindowHandle() => UnsafeNative.GetConsoleWindow();

        /// <summary>
        /// Hides the application's console
        /// </summary>
        public static bool HideConsole()
        {
            try
            {
                var handle = UnsafeNative.GetConsoleWindow();

                if (handle == null || handle == IntPtr.Zero)
                    return false;

                return UnsafeNative.ShowWindow(handle, UnsafeNative.SW_HIDE);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Shows the application's console
        /// </summary>
        public static bool ShowConsole()
        {
            try
            {
                var handle = UnsafeNative.GetConsoleWindow();

                if (handle == null || handle == IntPtr.Zero)
                    return false;

                return UnsafeNative.ShowWindow(handle, UnsafeNative.SW_SHOW);
            }
            catch (Exception)
            {
                return false;
            }
        }

#endif

        /// <summary>
        /// Search for processes with the same <see cref="Process.ProcessName"/> as the process defined by <paramref name="process"/>
        /// and blocks current thread until all current running instances has exited
        /// </summary>
        /// <param name="process">The process to wait for</param>
        public static void WaitForExit(Process process)
        {
            // We will only check the instances once
            // This way we create some sort of a queue

            var getCurrentWithTheSameName = new Func<IEnumerable<Process>>(() =>
                    Process.GetProcessesByName(process.ProcessName)
                        .Where(x => x.Id != process.Id));

            var instancesOfItself = getCurrentWithTheSameName();

            if (instancesOfItself.Any())
                foreach (var otherProcess in instancesOfItself)
                    if (!otherProcess.HasExited)
                        try
                        {
                            otherProcess.WaitForExit();
                        }
                        catch
                        {
                            // Might happen between because of the small time gap between HasExited and WaitForExit
                            // An Exception might occure, but can be ignored because an it can only mean that
                            // the process has exited
                        }
        }

        /// <summary>
        /// Search for all processes with the same name as <paramref name="processName"/>
        /// and blocks current thread until all current running instances has exited
        /// </summary>
        /// <param name="processName">The process name of the process to search for</param>
        public static void WaitForExitAll(string processName)
        {
            var currentProcess = Process.GetCurrentProcess();

            var getProcess = new Func<Process>(() =>
                    Process.GetProcessesByName(processName)
                        .FirstOrDefault(x => x.Id != currentProcess.Id));

            var process = getProcess();

            while (process != null)
            {
                try
                {
                    if (!process.HasExited)
                        process.WaitForExit();
                }
                catch
                {
                    // Might happen between because of the small time gap between HasExited and WaitForExit
                    // An Exception might occure, but can be ignored because an it can only mean that
                    // the process has exited
                }

                process = getProcess();
            };
        }

        #region WriteTable stuff

        /// <summary>
        /// Writes the specified table to the standard output stream
        /// </summary>
        /// <param name="columns">The table to write</param>
        /// <example>
        /// <code>
        /// var type = instance.GetType();
        /// var properties = type.GetPropertiesEx(BindingFlags.Public | BindingFlags.Instance);
        ///
        /// ConsoleUtils.WriteTable(
        ///     new ConsoleTableColumn(properties.Select(x => x.Name)),
        ///     new ConsoleTableColumn(properties.Select(x => x.GetValue(instance)?.ToString())));
        /// </code>
        /// </example>
        public static void WriteTable(params ConsoleTableColumn[] columns)
        {
            if (columns.Length == 0)
                return;

            try
            {
                // The maximum width each column can have
                var maxWidth = (Console.WindowWidth - 1) / columns.Sum(x => x.Width);

                foreach (var column in columns)
                    column._width = (int)(maxWidth * column.Width);

                var totalLine = columns.Max(x => x.Values.Count);

                foreach (var col in columns)
                    col._text = new string[totalLine][];

                // set multiline and text wrap stuff
                for (int line = 0; line < totalLine; line++)
                {
                    for (int i = 0; i < columns.Length; i++)
                    {
                        if (columns[i].Values.Count <= line)
                        {
                            columns[i]._text[line] = new string[] { "" };
                            continue;
                        }

                        var text = columns[i].Values[line].GetLines();

                        if (!columns[i].WrapWords)
                            columns[i]._text[line] = new string[] { text.Length == 0 ? "" : text[0] };
                        else
                        {
                            var additionalLines = new List<string>();

                            foreach (var p in text)
                            {
                                if (p.Length <= columns[i]._width)
                                    additionalLines.Add(p);
                                else
                                    additionalLines.AddRange(p.WrapWords(columns[i]._width - 10));
                            }

                            columns[i]._text[line] = additionalLines.ToArray();
                        }
                    }
                }

                foreach (var col in columns)
                    col.Values.Clear();

                for (int line = 0; line < totalLine; line++)
                {
                    // we have the maximum sub lines of all columns in the current line
                    var maxlines = columns.Max(x => x._text[line].Length);

                    foreach (var col in columns)
                        col.Values.AddRange(col._text[line].Pad(maxlines));
                }

                totalLine = columns.Max(x => x.Values.Count);

                for (int line = 0; line < totalLine; line++)
                {
                    for (int i = 0; i < columns.Length; i++)
                    {
                        var output = columns[i].Values[line];
                        var alternativeColor = output.StartsWith("!!");
                        Console.ForegroundColor = alternativeColor ? columns[i].AlternativeForeground : columns[i].Foreground;
                        Console.BackgroundColor = columns[i].Background;
                        WriteAligned(alternativeColor ? output.Substring(2) : output, columns[i].Alignment, columns[i]._width, columns[i].Filler);
                    }
                    Console.Write("\n");
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                Console.ResetColor();
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
                    result.Add(currentLine.Join(" "));
                    currentLine.Clear();
                    totalLineLength = 0;
                }

                currentLine.Add(word);
                totalLineLength += word.Length;
            }

            result.Add(currentLine.Join(" "));
            return result.ToArray();
        }

        private static void WriteAligned(string value, ColumnAlignment alignment, int widthLength, char filler)
        {
            string valueToWrite = "";

            switch (alignment)
            {
                case ColumnAlignment.Left:
                    if (value.Length > widthLength - 2)
                        valueToWrite = value.Substring(0, widthLength - 2).PadRight(widthLength, filler);
                    else
                        valueToWrite = value.PadRight(widthLength, filler);
                    break;

                case ColumnAlignment.Center:
                    var text = value.Length > widthLength - 2 ? value.Substring(value.Length - widthLength - 1, widthLength - 1) : value;
                    var centerText = text.Length / 2;
                    var centerColumn = widthLength / 2;
                    var pad = centerColumn - centerText;

                    valueToWrite = text.PadRight(widthLength - pad, filler).PadLeft(widthLength, filler);
                    break;

                case ColumnAlignment.Right:
                    if (value.Length > widthLength - 2)
                        valueToWrite = value.Substring(0, widthLength - 2).PadLeft(widthLength, filler);
                    else
                        valueToWrite = value.PadLeft(widthLength, filler);
                    break;
            }

            Console.Write(valueToWrite);
        }

        #endregion WriteTable stuff
    }
}