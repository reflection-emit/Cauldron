using System.Windows;
using System.Windows.Input;

namespace Couldron.Behaviours
{
    public sealed class ForcePropertyUpdate : Behaviour<FrameworkElement>
    {
        private bool tabPressed;

        protected override void OnAttach()
        {
            this.AssociatedObject.KeyUp += AssociatedObject_KeyUp;
        }

        protected override void OnDetach()
        {
            this.AssociatedObject.KeyUp -= AssociatedObject_KeyUp;
        }

        private void AssociatedObject_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.AssociatedObject.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                this.AssociatedObject.Focus();
            }
        }
    }
}