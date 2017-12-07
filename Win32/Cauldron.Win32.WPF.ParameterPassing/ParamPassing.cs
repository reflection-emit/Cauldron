using Cauldron.Core;
using System;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.XAML
{
    /// <summary>
    /// Handles parameter passing
    /// </summary>
    /// <example>
    /// The following code shows an implementation example.
    /// <para/>
    /// The parameter passing has to be configured in the static main.
    /// <code>
    /// [STAThread]
    /// public static void Main(string[] args)
    /// {
    ///     ParamPassing.Configure(args, x =&gt;
    ///     {
    ///         x.IsSingleInstance = true;
    ///         x.ParameterPassedCallback = new Action&lt;string[]&gt;(p =>
    ///         {
    ///             Application.Current.MainWindow.Title = string.Join(" ", p);
    ///         });
    ///     });
    ///
    ///     try
    ///     {
    ///         new App().Run(new MainWindow());
    ///     }
    ///     catch (Exception e)
    ///     {
    ///         MessageBox.Show(e.Message);
    ///     }
    /// }
    /// </code>
    /// <para/>
    /// The main window of the application has to add a message hook.
    /// <code>
    /// public MainWindow()
    /// {
    ///     InitializeComponent();
    ///     this.Loaded += (s, e) => this.AddHookParameterPassing();
    /// }
    /// </code>
    /// </example>
    public static class ParamPassing
    {
        internal static ParameterPassingConfig @params;

        /// <summary>
        /// Gets a value that indicates if other instances of the application are already running.
        /// </summary>
        public static bool AreOtherInstanceActive => Processes.Length > 0;

        private static Process[] Processes
        {
            get
            {
                // If an application is being run by VS then it will have the .vshost suffix to its proc
                // name. We have to also check them
                var proc = Process.GetCurrentProcess();
                var processName = proc.ProcessName.Replace(".vshost", "");
                return Process.GetProcesses()
                    .Where(x => (x.ProcessName == processName || x.ProcessName == proc.ProcessName || x.ProcessName == proc.ProcessName + ".vshost") && x.Id != proc.Id)
                    .ToArray();
            }
        }

        /// <summary>
        /// Brings the thread that created the specified application into the foreground and activates the
        /// window. Keyboard input is directed to the window, and various visual cues are changed for
        /// the user. The system assigns a slightly higher priority to the thread that created the
        /// foreground window than it does to other threads.
        /// <para/>
        /// This will only activate the first application instance in the list, if multiple instances are active.
        /// </summary>
        /// <returns>Returns true if successfull; otherwise false</returns>
        public static bool BringToFront()
        {
            var process = Processes.FirstOrDefault(x => x.MainWindowHandle != IntPtr.Zero);

            if (process != null)
            {
                UnsafeNative.ActivateWindow(process.MainWindowHandle);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Brings the thread that created the specified application into the foreground and activates the
        /// window. Keyboard input is directed to the window, and various visual cues are changed for
        /// the user. The system assigns a slightly higher priority to the thread that created the
        /// foreground window than it does to other threads.
        /// </summary>
        /// <param name="selector">If returns true, the window is activated.</param>
        public static void BringToFront(Func<Process, bool> selector)
        {
            foreach (var item in Processes.Where(x => x.MainWindowHandle != IntPtr.Zero))
                if (selector(item))
                    UnsafeNative.ActivateWindow(item.MainWindowHandle);
        }

        /// <summary>
        /// Configures the parameter passing
        /// </summary>
        /// <param name="args">The arguments passed to the application.</param>
        /// <param name="config">A delegate used to configure the parameter passing.</param>
        public static void Configure(string[] args, Action<ParameterPassingConfig> config)
        {
            var processes = Processes;
            @params = new ParameterPassingConfig(processes);
            config(@params);

            if (@params.Instances.Length == 0)
                return; // In this case we just proceed on loading the program

            // Single instance true will overrule everything else
            if (@params.IsSingleInstance)
            {
                if (args.Length > 0)
                    UnsafeNative.SendMessage(processes[0].MainWindowHandle, string.Join(@params.DataSeparator.ToString(), args));

                @params.ExitDelegate();
                return;
            }

            // You may want to have some parameters passed and some not
            if (@params.ValidationDelegate != null && !@params.ValidationDelegate(args))
                return;

            // Down here every empty args array will cause the to load regardless the config
            if (args.Length == 0)
                return;

            // If the process to prefer property is not null then we will only send the params to that certain process
            if (@params.ProcessToPrefer != null)
            {
                UnsafeNative.SendMessage(@params.ProcessToPrefer.MainWindowHandle, string.Join(@params.DataSeparator.ToString(), args));
                @params.ExitDelegate();
                return;
            }

            if (@params.Instances.Length == 1)
            {
                UnsafeNative.SendMessage(@params.Instances[0].MainWindowHandle, string.Join(@params.DataSeparator.ToString(), args));
                @params.ExitDelegate();
                return;
            }
            else if (@params.PassToAllInstances)
            {
                for (int i = 0; i < @params.Instances.Length; i++)
                    UnsafeNative.SendMessage(@params.Instances[i].MainWindowHandle, string.Join(@params.DataSeparator.ToString(), args));

                @params.ExitDelegate();
                return;
            }
            else if (@params.RandomSelectInstance)
            {
                UnsafeNative.SendMessage(Randomizer.Next(@params.Instances).MainWindowHandle, string.Join(@params.DataSeparator.ToString(), args));
                @params.ExitDelegate();
                return;
            }
        }
    }
}