using Cauldron.Behaviours;
using System.Windows.Controls;

namespace Cauldron.Themes.VisualStudio.Behaviours
{
    internal sealed class DatePickerBehaviour : Behaviour<DatePicker>
    {
        protected override void OnAttach()
        {
            this.AssociatedObject.SelectedDateChanged += AssociatedObject_SelectedDateChanged;
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