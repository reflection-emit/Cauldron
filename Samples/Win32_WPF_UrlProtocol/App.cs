using Cauldron.Core.Reflection;
using Cauldron.XAML;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace Win32_WPF_UrlProtocol
{
    public class App : ApplicationBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                App.ApplicationUrlProtocols.Add(@"cauldron");
                new App().Run();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, ApplicationInfo.ApplicationName + ": Unexpected error");
            }
        }

        protected override void OnActivated(string[] argument)
        {
        }

        protected override async void OnActivationProtocol(Uri uri)
        {
            var parsedUrl = HttpUtility.ParseQueryString(uri.Query);
            await this.MessageDialog.ShowOKAsync(parsedUrl.Get("title"), parsedUrl.Get("message"));
        }

        protected override Task OnStartup(LaunchActivatedEventArgs e) => this.Navigator.NavigateAsync<MainViewModel>();
    }
}