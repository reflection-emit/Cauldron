using Cauldron.Core.Reflection;
using Cauldron;
using System.Diagnostics;
using System.Windows;

namespace Win32_WPF_ParameterPassing
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // This is only meant for Lazy people
            // So that they don't have to open the console separately
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                WorkingDirectory = ApplicationInfo.ApplicationPath.FullName
            });
            this.Unloaded += (s, e) => process.Kill();

            // This is the only relevant line here.
            // This can be also added via Behaviour / Action
            // As long as the Window is loaded and a Window handle exists,
            // then everything is fine.
            this.Loaded += (s, e) => this.AddHookParameterPassing();
        }
    }
}
