using Couldron.Attached;
using Couldron.Behaviours;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Couldron.Themes.VisualStudio.Behaviours
{
    internal sealed class ComboBoxTreeView : Behaviour<TreeView>
    {
        private ComboBox comboBox;
        private Popup popup;

        protected override void OnAttach()
        {
            var logicalParent = this.AssociatedObject.FindLogicalParent<Grid>();
            this.comboBox = logicalParent?.FindVisualParent<ComboBox>();

            this.popup = this.AssociatedObject.FindLogicalParent<Popup>();

            if (this.popup != null)
            {
                this.popup.Opened += (s, e) =>
                {
                    if (this.AssociatedObject.ItemsSource == null)
                        return;

                    this.SetSelectedItems(ComboBoxProperties.GetSelectedItems(this.comboBox), this.AssociatedObject.ItemsSource);
                };

                this.popup.Closed += (s, e) =>
                {
                    if (this.AssociatedObject.ItemsSource == null)
                        return;

                    var selectedItems = new List<object>();
                    this.GetSelectedItems(selectedItems, this.AssociatedObject.ItemsSource);
                    ComboBoxProperties.SetSelectedItems(this.comboBox, selectedItems);
                };
            }
        }

        protected override void OnDetach()
        {
        }

        private void GetSelectedItems(IList listOfSelected, IEnumerable items)
        {
            if (items == null)
                return;

            foreach (var item in items)
            {
                var itemType = item.GetType();
                var children = itemType.GetProperty("Items").GetValue(item) as ICollection;

                if (children != null && children.Count() > 0)
                {
                    GetSelectedItems(listOfSelected, children);
                    continue;
                }

                itemType.GetProperty(this.AssociatedObject.SelectedValuePath).IsNotNull(x =>
                {
                    if (x.GetValue(item).ToBool())
                        listOfSelected.Add(item);
                });
            }
        }

        private void SetSelectedItems(IEnumerable selectedItems, IEnumerable items)
        {
            if (items == null)
                return;

            foreach (var item in items)
            {
                var itemType = item.GetType();
                var children = itemType.GetProperty("Items").GetValue(item) as ICollection;

                if (children != null && children.Count() > 0)
                {
                    SetSelectedItems(selectedItems, children);
                    continue;
                }

                itemType.GetProperty(this.AssociatedObject.SelectedValuePath).IsNotNull(x =>
                {
                    x.SetValue(item, selectedItems.Any(o => o == item));
                });
            }
        }
    }

    internal sealed class ComboBoxTreeViewSelectedElementContextMenu : Behaviour<Button>
    {
        private ComboBox comboBox;
        private MenuItem deleteMenuItem;

        public ComboBoxTreeViewSelectedElementContextMenu()
        {
            var delete = "Delete";

            if (Factory.HasContract(typeof(ILocalizationSource)))
            {
                var localization = Factory.Create<Localization>();
                delete = localization[delete];
            }

            this.deleteMenuItem = new MenuItem { Header = delete };
        }

        protected override void OnAttach()
        {
            this.comboBox = this.AssociatedObject.FindVisualParent<ComboBox>();

            this.AssociatedObject.ContextMenu = new ContextMenu();
            this.AssociatedObject.ContextMenu.Items.Add(deleteMenuItem);
            this.deleteMenuItem.Click += DeleteMenuItem_Click;
        }

        protected override void OnDetach()
        {
            this.deleteMenuItem.Click -= DeleteMenuItem_Click;
        }

        private void DeleteMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.comboBox == null)
                return;

            var selectedItems = ComboBoxProperties.GetSelectedItems(this.comboBox);
            ComboBoxProperties.SetSelectedItems(this.comboBox, selectedItems.Remove(this.AssociatedObject.Content));
        }
    }
}