using System;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Cauldron.Controls
{
    public interface ITreeViewComboBoxSelectionLogic
    {
        void ChangeSelectionValue(object item, bool value);

        bool IsSelected(object item);
    }

    public sealed class DefaultTreeViewComboBoxSelectionLogic : ITreeViewComboBoxSelectionLogic
    {
        public void ChangeSelectionValue(object item, bool value)
        {
            var property = item.GetType().GetProperty("IsActive");

            if (property == null)
                return;

            property.SetValue(item, value);
        }

        public bool IsSelected(object item)
        {
            var property = item.GetType().GetProperty("IsActive");

            if (property == null)
                return false;

            return property.GetValue(item).ToBool();
        }
    }

    public class TreeViewComboBox : ComboBox
    {
        #region Dependency Property AlternativeText

        /// <summary>
        /// Identifies the <see cref="AlternativeText" /> dependency property
        /// </summary>
        public static readonly DependencyProperty AlternativeTextProperty = DependencyProperty.Register(nameof(AlternativeText), typeof(string), typeof(TreeViewComboBox), new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets the <see cref="AlternativeText" /> Property
        /// </summary>
        public string AlternativeText
        {
            get { return (string)this.GetValue(AlternativeTextProperty); }
            set { this.SetValue(AlternativeTextProperty, value); }
        }

        #endregion Dependency Property AlternativeText

        #region Dependency Property Header

        /// <summary>
        /// Identifies the <see cref="Header" /> dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(TreeViewComboBox), new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets the <see cref="Header" /> Property
        /// </summary>
        public string Header
        {
            get { return (string)this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        #endregion Dependency Property Header

        #region Dependency Property SelectedItems

        /// <summary>
        /// Identifies the <see cref="SelectedItems" /> dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(nameof(SelectedItems), typeof(IEnumerable), typeof(TreeViewComboBox), new PropertyMetadata(null, TreeViewComboBox.OnSelectedItemsChanged));

        /// <summary>
        /// Gets or sets the <see cref="SelectedItems" /> Property
        /// </summary>
        public IEnumerable SelectedItems
        {
            get { return (IEnumerable)this.GetValue(SelectedItemsProperty); }
            set { this.SetValue(SelectedItemsProperty, value); }
        }

        private static void OnSelectedItemsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var d = dependencyObject as TreeViewComboBox;

            if (d == null)
                return;

            if ((args.NewValue as IEnumerable).Any())
                d.SelectedItem = (args.NewValue as IEnumerable).FirstElement();
        }

        #endregion Dependency Property SelectedItems

        #region Dependency Property SelectedItemTemplate

        /// <summary>
        /// Identifies the <see cref="SelectedItemTemplate" /> dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register(nameof(SelectedItemTemplate), typeof(DataTemplate), typeof(TreeViewComboBox), new PropertyMetadata(null, OnSelectedItemTemplateChanged));

        /// <summary>
        /// Gets or sets the <see cref="SelectedItemTemplate" /> Property
        /// </summary>
        public DataTemplate SelectedItemTemplate
        {
            get { return (DataTemplate)this.GetValue(SelectedItemTemplateProperty); }
            set { this.SetValue(SelectedItemTemplateProperty, value); }
        }

        private static void OnSelectedItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            //(args.NewValue as DataTemplate).IsNotNull(x => x.parent = d);
        }

        #endregion Dependency Property SelectedItemTemplate

        #region Dependency Property SelectionLogic

        /// <summary>
        /// Identifies the <see cref="SelectionLogic" /> dependency property
        /// </summary>
        public static readonly DependencyProperty SelectionLogicProperty = DependencyProperty.Register(nameof(SelectionLogic), typeof(ITreeViewComboBoxSelectionLogic), typeof(TreeViewComboBox), new PropertyMetadata(new DefaultTreeViewComboBoxSelectionLogic()));

        /// <summary>
        /// Gets or sets the <see cref="SelectionLogic" /> Property
        /// </summary>
        public ITreeViewComboBoxSelectionLogic SelectionLogic
        {
            get { return (ITreeViewComboBoxSelectionLogic)this.GetValue(SelectionLogicProperty); }
            set { this.SetValue(SelectionLogicProperty, value); }
        }

        #endregion Dependency Property SelectionLogic

        #region Dependency Property SelectedItemDisplayMemberPath

        /// <summary>
        /// Identifies the <see cref="SelectedItemDisplayMemberPath" /> dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedItemDisplayMemberPathProperty = DependencyProperty.Register(nameof(SelectedItemDisplayMemberPath), typeof(string), typeof(TreeViewComboBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="SelectedItemDisplayMemberPath" /> Property
        /// </summary>
        public string SelectedItemDisplayMemberPath
        {
            get { return (string)this.GetValue(SelectedItemDisplayMemberPathProperty); }
            set { this.SetValue(SelectedItemDisplayMemberPathProperty, value); }
        }

        #endregion Dependency Property SelectedItemDisplayMemberPath

        public TreeViewComboBox()
        {
        }

        public void RemoveSelectedItem(object selectedItem)
        {
            this.SelectedItems = this.SelectedItems.RemoveElement(selectedItem);
        }
    }
}