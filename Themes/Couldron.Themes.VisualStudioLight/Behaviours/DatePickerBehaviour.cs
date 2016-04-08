using Couldron.Attached;
using Couldron.Behaviours;
using Couldron.Validation;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace Couldron.Themes.VisualStudio.Behaviours
{
    internal sealed class DatePickerBehaviour : Behaviour<DatePicker>
    {
        protected override void OnAttach()
        {
            this.AssociatedObject.SelectedDateChanged += AssociatedObject_SelectedDateChanged;
        }

        protected override void OnDataContextChanged()
        {
            var binding = BindingOperations.GetBinding(this.AssociatedObject, DatePicker.SelectedDateProperty);

            if (binding == null)
                return;

            var path = binding.Path.Path;

            // get the property info
            this.AssociatedObject.DataContext.GetType().GetProperty(path)
                .IsNotNull(x => ValidationProperties.SetIsMandatory(this.AssociatedObject, x.GetCustomAttribute<IsMandatoryAttribute>() != null));
        }

        protected override void OnDetach()
        {
            this.AssociatedObject.SelectedDateChanged -= AssociatedObject_SelectedDateChanged;
        }

        private void AssociatedObject_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.AssociatedObject.IsDropDownOpen = false;
        }
    }
}