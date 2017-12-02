using Cauldron.Core.Extensions;
using Cauldron.XAML.Interactivity;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cauldron.XAML.Theme
{
    internal class ListViewBehaviour : Behaviour<ListView>
    {
        #region Dependency Property View

        /// <summary>
        /// Identifies the <see cref="View" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register(nameof(View), typeof(ViewBase), typeof(ListViewBehaviour), new PropertyMetadata(null, ListViewBehaviour.OnViewChanged));

        /// <summary>
        /// Gets or sets the <see cref="View" /> Property
        /// </summary>
        public ViewBase View
        {
            get { return (ViewBase)this.GetValue(ViewProperty); }
            set { this.SetValue(ViewProperty, value); }
        }

        private static void OnViewChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var gridView = args.NewValue as GridView;

            if (gridView == null)
                return;

            var behaviour = new GridViewColumnHeaderBehaviours();
            var behaviourCollection = Interaction.GetBehaviours(gridView);

            if (behaviourCollection == null)
                return;

            behaviourCollection.Add(behaviour);
        }

        #endregion Dependency Property View

        protected override void OnAttach()
        {
            this.AssociatedObject.ItemContainerStyle = new Func<Style>(() =>
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
            })();
            this.AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
            this.SetBinding(ListViewBehaviour.ViewProperty, this.AssociatedObject, new PropertyPath("View"), BindingMode.OneWay);
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
    }
}