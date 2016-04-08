using Couldron.Attached;
using Couldron.Behaviours;
using System.Windows.Controls;

namespace Couldron.Themes.VisualStudio.Behaviours
{
    internal sealed class DatePickerBehaviour : Behaviour<DatePicker>
    {
        protected override void OnAttach()
        {
            this.AssociatedObject.SelectedDateChanged += AssociatedObject_SelectedDateChanged;
        }

        protected override void OnDetach()
        {
        }

        private void AssociatedObject_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.AssociatedObject.IsDropDownOpen = false;
            //  UIElementProperties.SetBoolean(this.AssociatedObject, false);
        }
    }
}