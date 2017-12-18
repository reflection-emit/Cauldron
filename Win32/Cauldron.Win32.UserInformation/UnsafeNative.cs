using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Cauldron.Core
{
    internal static class UnsafeNative
    {
        public const int WTS_CURRENT_SESSION = -1;

        internal enum WTS_INFO_CLASS
        {
            WTSInitialProgram,
            WTSApplicationName,
            WTSWorkingDirectory,
            WTSOEMId,
            WTSSessionId,
            WTSUserName,
            WTSWinStationName,
            WTSDomainName,
            WTSConnectState,
            WTSClientBuildNumber,
            WTSClientName,
            WTSClientDirectory,
            WTSClientProductId,
            WTSClientHardwareId,
            WTSClientAddress,
            WTSClientDisplay,
            WTSClientProtocolType,
            WTSIdleTime,
            WTSLogonTime,
            WTSIncomingBytes,
            WTSOutgoingBytes,
            WTSIncomingFrames,
            WTSOutgoingFrames,
            WTSClientInfo,
            WTSSessionInfo
        }

        [DllImport("shell32.dll", EntryPoint = "#261", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void GetUserTilePath(string username, uint whatever, StringBuilder picpath, int maxLength);

        /// <summary>
        /// Gets the path of the user's profile picture
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The full path of the user's profile picture</returns>
        public static string GetUserTilePath(string username)
        {
            try
            {
                var stringBuilder = new StringBuilder(1000);
                UnsafeNative.GetUserTilePath(username, 0x80000000, stringBuilder, stringBuilder.Capacity);
                return stringBuilder.ToString();
            }
            catch
            {
                throw;
            }
        }

        [DllImport("wtsapi32.dll", ExactSpelling = true, SetLastError = false)]
        public static extern void WTSFreeMemory(IntPtr memory);

        [DllImport("Wtsapi32.dll")]
        public static extern bool WTSQuerySessionInformation(System.IntPtr hServer, int sessionId, UnsafeNative.WTS_INFO_CLASS wtsInfoClass, out System.IntPtr ppBuffer, out uint pBytesReturned);
    }
}