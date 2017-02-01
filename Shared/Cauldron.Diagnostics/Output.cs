#define DEBUG

using System.Diagnostics;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides a set of methods that help to output information of a compiled dll
    /// </summary>
    internal static class Output
    {
        public static void WriteLineError(string format, params object[] args) =>
            Debug.WriteLine("Error: " + format, args);

        public static void WriteLineInfo(string format, params object[] args) =>
            Debug.WriteLine("Information: " + format, args);
    }
}