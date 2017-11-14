using Cauldron.Win32.WPF.ParameterPassing;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System;

namespace ParameterPassingSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                WorkingDirectory = Path.GetDirectoryName(typeof(MainWindow).Assembly.Location)
            });

            this.Loaded += (s, e) => this.AddHook();
            this.Unloaded += (s, e) => process.Kill();
        }
    }
}