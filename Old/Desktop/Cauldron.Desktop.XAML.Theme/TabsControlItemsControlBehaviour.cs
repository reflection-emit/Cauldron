using Cauldron.XAML.Interactivity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cauldron.XAML.Theme
{
    internal class TabsControlItemsControlBehaviour : Behaviour<ItemsControl>
    {
        private string displayMemberPath;
        private bool isAttached = false;

        protected override void OnAttach()
        {
            if (this.isAttached)
                return;

            this.displayMemberPath = this.AssociatedObject.DisplayMemberPath;
            this.AssociatedObject.DisplayMemberPath = null;
            this.AssociatedObject.ItemTemplate = this.CreateDateTemplate();

            this.isAttached = true;
        }

        protected override void OnDetach()
        {
        }

        private static DependencyObject GetTabControl<T>(FrameworkElement element) where T : FrameworkElement
        {
            if (element == null)
                return null;

            if (element.Parent == null)
                return element.GetVisualParent();
            if (element.Parent.GetType() == typeof(T))
                return element.Parent;
            else
                return GetTabControl<T>(element.Parent as FrameworkElement);
        }

        private DataTemplate CreateDateTemplate()
        {
            var buttonFactory = new FrameworkElementFactory();

            buttonFactory.Type = typeof(Button);
            buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler((s, e) =>
            {
                var button = s as Button;
                if (button == null)
                    return;

                var tabControl = GetTabControl<TabControl>(this.AssociatedObject) as TabControl;
                if (tabControl == null)
                    return;

                tabControl.SelectedItem = button.DataContext;
            }));
            buttonFactory.SetValue(Button.HorizontalContentAlignmentProperty, HorizontalAlignment.Left);
            buttonFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            buttonFactory.SetValue(FrameworkElement.StyleProperty, Application.Current.Resources["TitleBarButtonStyle"] as Style);
            buttonFactory.SetBinding(Button.ContentProperty, new Binding
            {
                Path = new PropertyPath(this.displayMemberPath),
            });

            var result = new DataTemplate();
            result.VisualTree = buttonFactory;
            return result;
        }
    }
}