using Cauldron.Core;
using Cauldron.XAML;
using SingleInstanceApplicationWithParameterPassing.ViewModels;
using System.Threading.Tasks;

namespace SingleInstanceApplicationWithParameterPassing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            this.IsSingleInstance = true;
            this.ShouldBringToFront = false;
        }

        protected override void OnActivated(string[] argument)
        {
            MessageManager.Send(new ParameterMessagingArgs(this, argument));
        }

        protected override async Task OnStartup(LaunchActivatedEventArgs e)
        {
            await this.Navigator.NavigateAsync<MainViewModel>(new object[] { e.Arguments });
        }
    }
}