using Couldron.Behaviours;
using Couldron.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Couldron.Themes.VisualStudio.Behaviours
{
    internal sealed class ComboBoxTreeViewBehaviours : Behaviour<TreeView>
    {
        private TreeViewComboBox comboBox;
        private Popup popup;

        #region Dependency Property SelectionLogic

        /// <summary>
        /// Identifies the <see cref="SelectionLogic" /> dependency property
        /// </summary>
        public static readonly DependencyProperty SelectionLogicProperty = DependencyProperty.Register(nameof(SelectionLogic), typeof(ITreeViewComboBoxSelectionLogic), typeof(ComboBoxTreeViewBehaviours), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="SelectionLogic" /> Property
        /// </summary>
        public ITreeViewComboBoxSelectionLogic SelectionLogic
        {
            get { return (ITreeViewComboBoxSelectionLogic)this.GetValue(SelectionLogicProperty); }
            set { this.SetValue(SelectionLogicProperty, value); }
        }

        #endregion Dependency Property SelectionLogic

        protected override void OnAttach()
        {
            var logicalParent = this.AssociatedObject.FindLogicalParent<Grid>();
            this.comboBox = logicalParent?.FindVisualParent<TreeViewComboBox>();

            this.SelectionLogic = this.comboBox.SelectionLogic;

            this.popup = this.AssociatedObject.FindLogicalParent<Popup>();

            if (this.popup != null)
            {
                this.popup.Opened += (s, e) =>
                {
                    if (this.AssociatedObject.ItemsSource == null)
                        return;

                    this.SetSelectedItems(this.comboBox.SelectedItems, this.AssociatedObject.ItemsSource);
                };

                this.popup.Closed += (s, e) =>
                {
                    if (this.AssociatedObject.ItemsSource == null)
                        return;

                    var selectedItems = new List<object>();
                    this.GetSelectedItems(selectedItems, this.AssociatedObject.ItemsSource);
                    this.comboBox.SelectedItems = selectedItems;
                };
            }
        }

        protected override void OnDetach()
        {
        }

        private void GetSelectedItems(IList listOfSelected, IEnumerable items)
        {
            if (items == null || this.SelectionLogic == null)
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

                if (this.SelectionLogic.IsSelected(item))
                    listOfSelected.Add(item);
            }
        }

        private void SetSelectedItems(IEnumerable selectedItems, IEnumerable items)
        {
            if (items == null || this.SelectionLogic == null)
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

                this.SelectionLogic.ChangeSelectionValue(item, selectedItems.Any(o => o == item));
            }
        }
    }
}