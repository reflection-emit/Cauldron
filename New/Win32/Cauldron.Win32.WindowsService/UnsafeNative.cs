using System;
using System.Runtime.InteropServices;

namespace Cauldron.WindowsService
{
    internal static class UnsafeNative
    {
        public const int ERROR_ACCESS_DENIED = 5;
        public const int SC_MANAGER_ALL_ACCESS = 0xF003F;
        public const int SE_PRIVILEGE_ENABLED = 2;
        public const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        public const int SERVICE_ALL_ACCESS = 0xF01FF;
        public const int SERVICE_CONFIG_FAILURE_ACTIONS = 0x2;
        public const int TOKEN_ADJUST_PRIVILEGES = 32;
        public const int TOKEN_QUERY = 8;

        [DllImport("advapi32.dll")]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, [MarshalAs(UnmanagedType.Struct)] ref TokenPrivileges NewState, int BufferLength, IntPtr PreviousState, ref int ReturnLength);

        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2")]
        public static extern bool ChangeServiceFailureActions(IntPtr hService, int dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref ServiceFailureActions lpInfo);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hndl);

        [DllImport("advapi32.dll")]
        public static extern bool CloseServiceHandle(IntPtr hSCObject);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        [DllImport("advapi32.dll")]
        public static extern IntPtr LockServiceDatabase(IntPtr hSCManager);

        [DllImport("advapi32.dll")]
        public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, ref long lpLuid);

        [DllImport("advapi32.dll")]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

        [DllImport("advapi32.dll")]
        public static extern IntPtr OpenSCManager(string lpMachineName, string lpDatabaseName, int dwDesiredAccess);

        [DllImport("advapi32.dll")]
        public static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, int dwDesiredAccess);

        [DllImport("advapi32.dll")]
        public static extern bool UnlockServiceDatabase(IntPtr hSCManager);

        [StructLayout(LayoutKind.Sequential)]
        public struct LuidAndAttributes
        {
            public long Luid;
            public int Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceFailureActions
        {
            #region Hack For CodeMaid

            public int dwResetPeriod;

            #endregion Hack For CodeMaid

            #region Hack For CodeMaid

            public string lpRebootMsg;

            #endregion Hack For CodeMaid

            #region Hack For CodeMaid

            public string lpCommand;

            #endregion Hack For CodeMaid

            #region Hack For CodeMaid

            public int cActions;

            #endregion Hack For CodeMaid

            #region Hack For CodeMaid

            public int lpsaActions;

            #endregion Hack For CodeMaid
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TokenPrivileges
        {
            public int PrivilegeCount;
            public LuidAndAttributes Privileges;
        }
    }
}