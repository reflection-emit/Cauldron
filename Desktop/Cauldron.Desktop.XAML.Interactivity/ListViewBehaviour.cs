using Cauldron.XAML.Interactivity.Attached;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cauldron.XAML.Interactivity
{
    internal sealed class ListViewBehaviour : Behaviour<ListView>
    {
        protected override void OnAttach()
        {
            this.AssociatedObject.ItemContainerStyle = this.CreateStyle();
            this.AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        protected override void OnDetach()
        {
            this.AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItems = this.AssociatedObject.SelectedItems as IEnumerable;
            ListViewProperties.SetSelectedItems(this.AssociatedObject, selectedItems);
        }

        private Style CreateStyle()
        {
            var style = new Style(typeof(ListViewItem));
            style.BasedOn = (Style)Application.Current.FindResource(typeof(ListViewItem));

            var mouseDoubleClickEventTrigger = new EventSetter(ListViewItem.MouseDoubleClickEvent,
                new MouseButtonEventHandler((s, e) =>
                {
                    var sender = (s as ListViewItem).DataContext;
                    var command = ListViewProperties.GetCommand(this.AssociatedObject);
                    if (command != null && command.CanExecute(sender))
                        command.Execute(sender);

                    (e as MouseButtonEventArgs).Handled = true;
                }));

            var mouseKeyUpEventTrigger = new EventSetter(ListViewItem.KeyDownEvent,
                new KeyEventHandler((s, e) =>
                {
                    var args = e as KeyEventArgs;

                    if (args.Key == Key.Enter && args.SystemKey == Key.None)
                    {
                        var sender = (s as ListViewItem).DataContext;
                        var command = ListViewProperties.GetCommand(this.AssociatedObject);
                        if (command != null && command.CanExecute(sender))
                            command.Execute(sender);

                        args.Handled = true;
                    }
                }));

            style.Setters.Add(mouseDoubleClickEventTrigger);
            style.Setters.Add(mouseKeyUpEventTrigger);

            return style;
        }
    }
}