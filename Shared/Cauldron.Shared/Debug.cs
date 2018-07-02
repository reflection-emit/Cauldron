#define DEBUG

using System;
using System.Text;

#if WINDOWS_UWP

using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;

#else

using System.Net.NetworkInformation;
using System.Security.Cryptography;

#endif

namespace Cauldron.Core.Diagnostics
{
    /// <summary>
    /// Provides a set of methods that helps debug code.
    /// <para/>
    /// These methods are not DEBUG conditional and will stay in Release build. Use them with care.
    /// </summary>
#if PUBLIC
    public static class Debug
#else

    internal static class Debug

#endif
    {
        /// <summary>
        /// Returns a string that can be used to identify the hardware
        /// </summary>
        public static string HardwareIdentifier
        {
            get
            {
#if NETFX_CORE
                var token = HardwareIdentification.GetPackageSpecificToken(null);
                var id = token.Id;
                var bytes = new byte[id.Length];

                using (var reader = DataReader.FromBuffer(id))
                    reader.ReadBytes(bytes);

                return GetHash(Convert.ToBase64String(bytes));
#else
                // TODO - This may not be enough to uniquely identify a hardware...
                var sb = new StringBuilder();

                foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
                    sb.Append(adapter.GetPhysicalAddress().ToString());

                sb.Append(Environment.MachineName);
                sb.Append(Environment.ProcessorCount);
                sb.Append(Environment.OSVersion);

                return GetHash(sb.ToString());
#endif
            }
        }

        /// <summary>
        /// Gets the stacktrace of the exception and the inner exceptions recursively
        /// </summary>
        /// <param name="e">The exception with the stack trace</param>
        /// <returns>A string representation of the stacktrace</returns>
        public static string GetStackTrace(this Exception e)
        {
            var sb = new StringBuilder();
            var ex = e;

            do
            {
                sb.AppendLine("Exception Type: " + ex.GetType().Name);
                sb.AppendLine("Source: " + ex.Source);
                sb.AppendLine(ex.Message);
                sb.AppendLine("------------------------");
                sb.AppendLine(ex.StackTrace);
                sb.AppendLine("------------------------");

                ex = ex.InnerException;
            } while (ex != null);

            return sb.ToString();
        }

        /// <summary>
        /// Writes a formatted message followed by a line terminator to the trace listeners in the
        /// System.Diagnostics.Debug.Listeners collection.
        /// </summary>
        /// <param name="format">
        /// A composite format string that contains text intermixed with zero or more format items,
        /// which correspond to objects in the args array.
        /// </param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void WriteLine(string format, params object[] args) => System.Diagnostics.Debug.WriteLine(format, args);

        /// <summary>
        /// Writes a message followed by a line terminator to the trace listeners in the
        /// System.Diagnostics.Debug.Listeners collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public static void WriteLine(string message) => System.Diagnostics.Debug.WriteLine(message);

        /// <summary>
        /// Writes the full stack-trace of an exception to the trace listeners in the
        /// System.Diagnostics.Debug.Listeners collection.
        /// </summary>
        /// <param name="e">The exception to write</param>
        public static void WriteLine(Exception e) => System.Diagnostics.Debug.WriteLine(e.GetStackTrace());

        private static string GetHash(string value)
        {
#if WINDOWS_UWP
            var sha = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var buffer = CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(value));

            var hashed = sha.HashData(buffer);
            byte[] bytes;

            CryptographicBuffer.CopyToByteArray(hashed, out bytes);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
#else
            using (var sha = SHA256.Create())
                return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(value)));

#endif
        }
    }
}