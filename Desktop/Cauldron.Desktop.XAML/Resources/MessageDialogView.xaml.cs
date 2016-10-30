using System.Windows.Input;

#if WINDOWS_UWP
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#else

using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

#endif

namespace Cauldron.XAML.Resources
{
    internal sealed partial class MessageDialogView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MessageDialogView"/>
        /// </summary>
        public MessageDialogView()
        {
            this.InitializeComponent();
        }
    }
}