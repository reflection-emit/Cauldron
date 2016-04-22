using Cauldron.Behaviours;
using System.Windows.Controls;

namespace Cauldron.Themes.VisualStudio.Behaviours
{
    internal class ComboBoxBehaviour : Behaviour<ComboBox>
    {
        protected override void OnAttach()
        {
            this.AssociatedObject.KeyUp += AssociatedObject_KeyUp;
        }

        protected override void OnDetach()
        {
            this.AssociatedObject.KeyUp -= AssociatedObject_KeyUp;
        }

        private void AssociatedObject_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
                this.AssociatedObject.SelectedItem = null;
            else if (e.Key == System.Windows.Input.Key.Space)
                this.AssociatedObject.IsDropDownOpen = true;
        }
    }
}