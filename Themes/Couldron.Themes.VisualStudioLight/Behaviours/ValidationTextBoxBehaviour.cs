using Couldron.Behaviours;
using Couldron.Validation;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Couldron.Themes.VisualStudio.Behaviours
{
    internal class ValidationTextBoxBehaviour : Behaviour<FrameworkElement>
    {
        protected override void OnAttach()
        {
        }

        protected override void OnDataContextChanged()
        {
            var binding = BindingOperations.GetBinding(this.AssociatedObject, TextBox.TextProperty);

            if (binding == null)
                return;

            var path = binding.Path.Path;

            // get the property info
            this.AssociatedObject.DataContext.GetType().GetProperty(path)
                .IsNotNull(x => ValidationProperties.SetIsMandatory(this.AssociatedObject, x.GetCustomAttribute<IsMandatoryAttribute>() != null));
        }

        protected override void OnDetach()
        {
        }
    }
}