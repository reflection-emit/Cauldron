using System.Security.Permissions;
using System.Windows.Threading;

namespace Couldron.Test
{
    /*
        http://stackoverflow.com/questions/1106881/using-the-wpf-dispatcher-in-unit-tests
    */

    public static class DispatcherUtil
    {
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        private static object ExitFrame(object frame)
        {
            (frame as DispatcherFrame).Continue = false;
            return null;
        }
    }
}