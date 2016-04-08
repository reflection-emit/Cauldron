using Couldron.Attached;
using Couldron.Behaviours;
using Couldron.Controls;
using Couldron.Validation;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Couldron.Themes.VisualStudio.Behaviours
{
    internal class ComboBoxBehaviour : Behaviour<ComboBox>
    {
        protected override void OnAttach()
        {
            this.AssociatedObject.KeyUp += AssociatedObject_KeyUp;
        }

        protected override void OnDataContextChanged()
        {
            DependencyProperty dependencyProperty = null;

            if (this.AssociatedObject.IsEditable)
                dependencyProperty = ComboBox.TextProperty;
            else
                dependencyProperty = ComboBox.SelectedItemProperty;

            var binding = BindingOperations.GetBinding(this.AssociatedObject, dependencyProperty);

            if (binding == null)
                return;

            var path = binding.Path.Path;

            // get the property info
            this.AssociatedObject.DataContext.GetType().GetProperty(path)
                .IsNotNull(x => ValidationProperties.SetIsMandatory(this.AssociatedObject, x.GetCustomAttribute<IsMandatoryAttribute>() != null));
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

    internal class TreeViewComboBoxBehaviour : Behaviour<ComboBox>
    {
        protected override void OnAttach()
        {
            this.AssociatedObject.KeyUp += AssociatedObject_KeyUp;
        }

        protected override void OnDataContextChanged()
        {
            var binding = BindingOperations.GetBinding(this.AssociatedObject, TreeViewComboBox.SelectedItemsProperty);

            if (binding == null)
                return;

            var path = binding.Path.Path;

            // get the property info
            this.AssociatedObject.DataContext.GetType().GetProperty(path)
                .IsNotNull(x => ValidationProperties.SetIsMandatory(this.AssociatedObject, x.GetCustomAttribute<IsMandatoryAttribute>() != null));
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