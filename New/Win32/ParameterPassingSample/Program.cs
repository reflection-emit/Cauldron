using Cauldron.Win32.WPF.ParameterPassing;
using System;
using System.Windows;

namespace ParameterPassingSample
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ParamPassing.Configure(args, x =>
            {
                x.IsSingleInstance = true;
                x.ParameterPassedCallback = new Action<string[]>(p =>
                {
                    Application.Current.MainWindow.Title = string.Join(" ", p);
                });
            });

            try
            {
                new App().Run(new MainWindow());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}