using Cauldron.Extensions;
using Cauldron.XAML.Interactivity;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Cauldron.XAML.Theme
{
    internal class TabControlDecorationBorderBehavior : Behaviour<Rectangle>
    {
        private TabControl tabControl;

        protected override void OnAttach()
        {
            this.tabControl = this.AssociatedObject.FindVisualParent<TabControl>();

            if (this.tabControl != null)
                this.tabControl.SelectionChanged += TabControl_SelectionChanged;

            this.ChangeColor();
        }

        protected override void OnDataContextChanged() => this.ChangeColor();

        protected override void OnDetach()
        {
            if (this.tabControl != null)
                this.tabControl.SelectionChanged -= TabControl_SelectionChanged;

            this.tabControl = null;
        }

        private void ChangeColor()
        {
            if (this.AssociatedObject == null || this.tabControl == null)
                return;

            var selectedInfo = this.tabControl.GetPropertyNonPublicValue<object>("InternalSelectedInfo");

            if (selectedInfo == null)
            {
                this.AssociatedObject.Fill = this.tabControl.Background;
                return;
            }

            var container = selectedInfo.GetPropertyNonPublicValue<TabItem>("Container");

            if (container != null)
                this.AssociatedObject.Fill = container.Background;
            else
                this.AssociatedObject.Fill = this.tabControl.Background;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != this.tabControl)
                return;

            this.ChangeColor();
        }
    }
}