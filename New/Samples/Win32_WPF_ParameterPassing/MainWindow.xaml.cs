using Cauldron.Core.Reflection;
using Cauldron.WPF.ParameterPassing;
using System.Diagnostics;
using System.Windows;

namespace Win32_WPF_ParameterPassing
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                WorkingDirectory = ApplicationInfo.ApplicationPath.FullName
            });

            this.Loaded += (s, e) => this.AddHook();
            this.Unloaded += (s, e) => process.Kill();
        }
    }
}