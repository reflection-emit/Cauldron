using System.Windows;
using System.Windows.Input;

namespace Couldron.Behaviours
{
    public sealed partial class EnterKeyToCommand : Behaviour<FrameworkElement>
    {
        private void AssociatedObject_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.Command == null)
                return;

            if (e.Key == Key.Enter)
            {
                if (this.Command.CanExecute(null))
                    this.Command.Execute(null);
            }
        }
    }
}