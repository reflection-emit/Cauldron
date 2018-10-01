using Cauldron.WindowsService;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace Cauldron
{
    /// <summary>
    /// Provides extensions for the service
    /// </summary>
    public static partial class ExtensionsWindowsService
    {
        /// <summary>
        /// Tries to start the service
        /// </summary>
        /// <param name="serviceController">The service controller to use.</param>
        public static void TryStartService(this ServiceController serviceController)
        {
            if (serviceController == null)
                throw new ArgumentNullException(nameof(serviceController));

            ServiceController(serviceController.ServiceName, x =>
            {
                if (x.Status != ServiceControllerStatus.Running)
                {
                    x.Start();
                    x.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }
            });
        }

        /// <summary>
        /// Tries to start the service
        /// </summary>
        /// <param name="serviceInstaller">The service installer used to install the service.</param>
        public static void TryStartService(this ServiceInstaller serviceInstaller)
        {
            if (serviceInstaller == null)
                throw new ArgumentNullException(nameof(serviceInstaller));

            ServiceController(serviceInstaller.ServiceName, x =>
            {
                if (x.Status != ServiceControllerStatus.Running)
                {
                    x.Start();
                    x.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }
            });
        }

        /// <summary>
        /// Tries to stop the service
        /// </summary>
        /// <param name="serviceController">The service controller to use.</param>
        public static void TryStopService(this ServiceController serviceController)
        {
            if (serviceController == null)
                throw new ArgumentNullException(nameof(serviceController));

            ServiceController(serviceController.ServiceName, x =>
            {
                if (x.Status != ServiceControllerStatus.Stopped)
                {
                    x.Stop();
                    x.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                }
            });
        }

        /// <summary>
        /// Tries to stop the service
        /// </summary>
        /// <param name="serviceInstaller">The service installer used to install the service.</param>
        public static void TryStopService(this ServiceInstaller serviceInstaller)
        {
            if (serviceInstaller == null)
                throw new ArgumentNullException(nameof(serviceInstaller));

            ServiceController(serviceInstaller.ServiceName, x =>
            {
                if (x.Status != ServiceControllerStatus.Stopped)
                {
                    x.Stop();
                    x.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                }
            });
        }

        internal static void SetServiceFailureActions(this ServiceInstaller serviceInstaller, FailureActions failureActions)
        {
            if (serviceInstaller == null)
                throw new ArgumentNullException(nameof(serviceInstaller));

            if (failureActions == null)
                throw new ArgumentNullException(nameof(failureActions));

            // If any of the of the actions is reboot,
            // we need to request shutdown priviledges from the system
            if (failureActions.FirstFailure == RecoveryAction.Reboot |
                failureActions.SecondFailure == RecoveryAction.Reboot |
                failureActions.SubsequentFailure == RecoveryAction.Reboot)
                RequestShutdownPrivilege();

            var actionsPointer = IntPtr.Zero;
            var serviceManager = IntPtr.Zero;
            var serviceLock = IntPtr.Zero;
            var serviceHandle = IntPtr.Zero;

            try
            {
                var restartServiceAfter = (int)failureActions.RestartServiceAfter.TotalMilliseconds;
                var actions = new int[6];
                // First failure
                actions[0] = (int)failureActions.FirstFailure;
                actions[1] = restartServiceAfter;
                // Second failure
                actions[2] = (int)failureActions.SecondFailure;
                actions[3] = restartServiceAfter;
                // subsequent failure
                actions[4] = (int)failureActions.SubsequentFailure;
                actions[5] = restartServiceAfter;

                actionsPointer = Marshal.AllocHGlobal(24 /* 3 x 2 x 4 */);
                Marshal.Copy(actions, 0, actionsPointer, 6);

                var serviceFailureActions = new UnsafeNative.ServiceFailureActions();
                serviceFailureActions.cActions = 3;
                serviceFailureActions.dwResetPeriod = (int)failureActions.ResetFailCountAfter.TotalSeconds;
                serviceFailureActions.lpCommand = $"{failureActions.RunProgram} {failureActions.RunProgramArguments}";
                serviceFailureActions.lpRebootMsg = string.Empty;
                serviceFailureActions.lpsaActions = actionsPointer.ToInt32();

                serviceManager = UnsafeNative.OpenSCManager(null, null, UnsafeNative.SC_MANAGER_ALL_ACCESS);

                if (serviceManager.ToInt32() <= 0)
                    throw new ServiceManagerException("Unable to open the service manager.");

                serviceLock = UnsafeNative.LockServiceDatabase(serviceManager);

                if (serviceLock.ToInt32() <= 0)
                    throw new ServiceManagerException("Unable to lock the service database.");

                serviceHandle = UnsafeNative.OpenService(serviceManager, serviceInstaller.ServiceName, UnsafeNative.SERVICE_ALL_ACCESS);

                if (serviceHandle.ToInt32() <= 0)
                    throw new ServiceManagerException($"Unable to open the '{serviceInstaller.ServiceName}' service.");

                if (!UnsafeNative.ChangeServiceFailureActions(serviceHandle, UnsafeNative.SERVICE_CONFIG_FAILURE_ACTIONS, ref serviceFailureActions))
                {
                    var lastError = UnsafeNative.GetLastError();
                    if (lastError == UnsafeNative.ERROR_ACCESS_DENIED)
                        throw new UnauthorizedAccessException("Access denied while setting failure actions.");

                    throw new ServiceManagerException("An error has occured while setting failure actions. Error code: " + lastError);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (serviceManager != IntPtr.Zero && serviceLock != IntPtr.Zero)
                    UnsafeNative.UnlockServiceDatabase(serviceLock);

                if (actionsPointer != IntPtr.Zero)
                    Marshal.FreeHGlobal(actionsPointer);

                if (serviceHandle != IntPtr.Zero)
                    UnsafeNative.CloseServiceHandle(serviceHandle);

                if (serviceManager != IntPtr.Zero)
                    UnsafeNative.CloseServiceHandle(serviceManager);
            }
        }

        private static void RequestShutdownPrivilege()
        {
            // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/sysinfo/base/shutting_down.asp
            // https://www.codeproject.com/Articles/6164/A-ServiceInstaller-Extension-That-Enables-Recovery

            var processToken = IntPtr.Zero;
            var currentProcess = Process.GetCurrentProcess().Handle;
            var tokenPrivileges = new UnsafeNative.TokenPrivileges();

            try
            {
                if (!UnsafeNative.OpenProcessToken(
                    currentProcess,
                    UnsafeNative.TOKEN_ADJUST_PRIVILEGES | UnsafeNative.TOKEN_QUERY,
                    ref processToken))
                    throw new AccessTokenException("Unable to open process token");

                long lpUid = 0;
                UnsafeNative.LookupPrivilegeValue(null, UnsafeNative.SE_SHUTDOWN_NAME, ref lpUid);

                tokenPrivileges.PrivilegeCount = 1;
                tokenPrivileges.Privileges.Luid = lpUid;
                tokenPrivileges.Privileges.Attributes = UnsafeNative.SE_PRIVILEGE_ENABLED;

                int returnLength = 0;
                UnsafeNative.AdjustTokenPrivileges(processToken, false, ref tokenPrivileges, 0, IntPtr.Zero, ref returnLength);

                if (UnsafeNative.GetLastError() != 0)
                    throw new UnauthorizedAccessException("Failed to grant shutdown privilege");
            }
            catch
            {
                throw;
            }
            finally
            {
                if (processToken != IntPtr.Zero)
                    UnsafeNative.CloseHandle(processToken);
            }
        }

        private static void ServiceController(string serviceName, Action<ServiceController> action)
        {
            var controller = new ServiceController(serviceName);
            try
            {
                action(controller);
            }
            catch
            {
                throw;
            }
            finally
            {
                controller.Close();
            }
        }
    }
}