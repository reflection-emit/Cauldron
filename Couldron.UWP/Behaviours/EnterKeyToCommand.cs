using Windows.System;
using Windows.UI.Xaml;

namespace Couldron.Behaviours
{
    public sealed partial class EnterKeyToCommand : Behaviour<FrameworkElement>
    {
        private void AssociatedObject_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (this.Command == null)
                return;

            if (e.Key == VirtualKey.Enter)
            {
                if (this.Command.CanExecute(null))
                    this.Command.Execute(null);
            }
        }
    }
}