#if WINDOWS_UWP
using Windows.UI.Xaml.Controls;
#else

using System.Windows.Controls;

#endif

namespace StandardApplication.Views
{
    public sealed partial class MainView : UserControl
    {
        public MainView()
        {
            this.InitializeComponent();
        }
    }
}