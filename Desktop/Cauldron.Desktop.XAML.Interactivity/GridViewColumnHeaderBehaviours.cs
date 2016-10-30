using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.Potions;
using Cauldron.XAML.Interactivity.Attached;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Windows.Storage;
using Windows.UI.Core;

namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// Provides a behaviour that implements column sorting, header width and position persisting
    /// </summary>
    public class GridViewColumnHeaderBehaviours : Behaviour<GridView>
    {
        private ColumnHeaderPropertiesCollection columnProperties = new ColumnHeaderPropertiesCollection();
        private DependencyObject inheritanceContext;
        private DynamicEventHandler inheritanceContextChangedHandler;
        private SortAdorner sortAdorner = null;
        private GridViewColumnHeader sortedHeader;

        #region Dependency Property IsSortable

        /// <summary>
        /// Identifies the <see cref="IsSortable" /> dependency property
        /// </summary>
        public static readonly DependencyProperty IsSortableProperty = DependencyProperty.Register(nameof(IsSortable), typeof(bool), typeof(GridViewColumnHeaderBehaviours), new PropertyMetadata(true, GridViewColumnHeaderBehaviours.OnIsSortableChanged));

        /// <summary>
        /// Gets or sets the <see cref="IsSortable" /> Property
        /// </summary>
        public bool IsSortable
        {
            get { return (bool)this.GetValue(IsSortableProperty); }
            set { this.SetValue(IsSortableProperty, value); }
        }

        private static void OnIsSortableChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var d = dependencyObject as GridViewColumnHeaderBehaviours;

            if (d == null)
                return;

            var value = args.NewValue?.ToString().ToBool();

            if (!value.HasValue || (value.HasValue && !value.Value))
            {
                if (d.sortedHeader != null && d.sortAdorner != null)
                    AdornerLayer.GetAdornerLayer(d.sortedHeader).Remove(d.sortAdorner);
            }
        }

        #endregion Dependency Property IsSortable

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
            this.AssociatedObject.AllowsColumnReorder = true;
            this.inheritanceContext = this.AssociatedObject.GetInheritanceContext();

            if (this.inheritanceContext == null)
                this.inheritanceContextChangedHandler = new DynamicEventHandler(this.AssociatedObject, "InheritanceContextChanged", (s, e) =>
                {
                    this.inheritanceContext = this.AssociatedObject.GetInheritanceContext();
                    this.inheritanceContextChangedHandler?.Dispose();

                    // The owner of this property is not a framework element therefor no Unloaded event
                    // We have to get this from the inheritanceContext
                    var element = this.inheritanceContext?.As<FrameworkElement>();
                    if (element != null)
                    {
                        element.Unloaded += Element_Unloaded;
                        element.DataContextChanged += Element_DataContextChanged;
                    }
                });

            // We have to be aware of application exit...
            // The unload event of the controls does not occure on application exit
            Application.Current.Exit += Current_Exit;
            Application.Current.SessionEnding += Current_SessionEnding;
        }

        /// <summary>
        /// Occures if the <see cref="FrameworkElement.DataContext"/> of <see cref="Behaviour{T}.AssociatedObject"/> has changed
        /// </summary>
        protected override void OnDataContextChanged()
        {
            base.OnDataContextChanged();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            DispatcherEx.Current.RunAsync(CoreDispatcherPriority.Low, () => this.Initialize());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
            Application.Current.Exit -= Current_Exit;
            Application.Current.SessionEnding -= Current_SessionEnding;

            var element = this.inheritanceContext?.As<FrameworkElement>();
            if (element != null)
            {
                element.DataContextChanged -= Element_DataContextChanged;
                element.Unloaded -= Element_Unloaded;
            }

            this.inheritanceContextChangedHandler?.Dispose();
            this.columnProperties.Columns.Clear();

            try
            {
                foreach (var column in this.AssociatedObject.Columns.ToArray())
                    columnProperties.Columns.Add(new ColumnHeaderProperties { Width = column.ActualWidth < 16 ? 16 : column.ActualWidth, Uid = (column.Header as GridViewColumnHeader).Uid });

                if (this.sortAdorner != null && this.sortedHeader != null)
                {
                    this.columnProperties.SortingDirection = this.sortAdorner.Direction;
                    this.columnProperties.SortedHeaderUid = this.sortedHeader.Uid;
                }

                Serializer.CreateInstance()?.Serialize(this.columnProperties, ApplicationData.Current.LocalFolder, this.GetSerializedUniqueName());
            }
            catch
            {
                // This can be a problem if the Unload event comes later than the session end or application exit event...
                // In this case the Header will be null and we will have a nice Null exceptiuon
                // But... We just skip this one... It does not matter actually... The columns are not persisted then... No problem
            }
        }

        private void ApplyPersistent()
        {
            int index = 0;

            // move the columns and apply the widths
            foreach (var p in this.columnProperties.Columns)
            {
                var column = this.AssociatedObject.Columns.FirstOrDefault(x => (x.Header as GridViewColumnHeader).Uid == p.Uid);

                if (column == null)
                    return;

                column.Width = p.Width;
                this.AssociatedObject.Columns.Move(this.AssociatedObject.Columns.IndexOf(column), index++);
            }
        }

        private DataTemplate CreateDateTemplate(BindingBase binding, GridViewColumnFormatting formatting)
        {
            var factory = new FrameworkElementFactory();
            var foreground = this.inheritanceContext.As<ListView>().Foreground;

            switch (formatting)
            {
                case GridViewColumnFormatting.TextLeft:
                    factory.Type = typeof(TextBlock);
                    factory.SetBinding(TextBlock.TextProperty, binding);
                    factory.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                    factory.SetValue(TextBlock.TextWrappingProperty, TextWrapping.NoWrap);
                    factory.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
                    factory.SetValue(TextBlock.ForegroundProperty, foreground);
                    break;

                case GridViewColumnFormatting.TextCenter:
                    factory.Type = typeof(TextBlock);
                    factory.SetBinding(TextBlock.TextProperty, binding);
                    factory.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                    factory.SetValue(TextBlock.TextWrappingProperty, TextWrapping.NoWrap);
                    factory.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
                    factory.SetValue(TextBlock.ForegroundProperty, foreground);
                    break;

                case GridViewColumnFormatting.TextRight:
                    factory.Type = typeof(TextBlock);
                    factory.SetBinding(TextBlock.TextProperty, binding);
                    factory.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right);
                    factory.SetValue(TextBlock.TextWrappingProperty, TextWrapping.NoWrap);
                    factory.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
                    factory.SetValue(TextBlock.ForegroundProperty, foreground);
                    break;

                case GridViewColumnFormatting.Boolean:
                    factory.Type = typeof(CheckBox);
                    factory.SetBinding(CheckBox.IsCheckedProperty, binding);
                    factory.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                    break;

                default:
                    break;
            }

            factory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            factory.SetValue(UIElement.IsHitTestVisibleProperty, false);

            var result = new DataTemplate();
            result.VisualTree = factory;
            return result;
        }

        private void CreateHeaders()
        {
            int index = 0;
            var dataContextType = this.inheritanceContext?.As<FrameworkElement>()?.DataContext?.GetType();

            foreach (var column in this.AssociatedObject.Columns)
            {
                // check the header if it contains a GridViewColumnHeader or not...
                // if not, then create a GridViewColumnHeader and assign the value to the GridViewColumnHeader
                var header = column.Header is GridViewColumnHeader ?
                    column.Header as GridViewColumnHeader :
                    new GridViewColumnHeader { Content = column.Header };

                header.Click += (s, args) => this.SetSorting(s as GridViewColumnHeader);
                header.Uid = $"index_{index++}";
                header.HorizontalContentAlignment = HorizontalAlignment.Center;
                column.Header = header;

                if (dataContextType == null)
                    continue;

                var memberBinding = column.DisplayMemberBinding as Binding;
                if (memberBinding == null)
                    continue;

                var path = memberBinding.Path.Path;
                if (path == null)
                    continue;

                GridViewColumnProperties.SetSortingPropertyName(header, path);

                column.DisplayMemberBinding = null; // Always remove the displaybinding if a member is defined

                if (column.CellTemplate != null)
                    continue;

                // auto formatting down here
                var formatting = GridViewColumnProperties.GetFormatting(column);
                column.CellTemplate = this.CreateDateTemplate(memberBinding, formatting);
            }
        }

        private void Current_Exit(object sender, ExitEventArgs e) => this.OnDetach();

        private void Current_SessionEnding(object sender, SessionEndingCancelEventArgs e) => this.OnDetach();

        private void Element_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) => this.OnDataContextChanged();

        private void Element_Unloaded(object sender, RoutedEventArgs e) => this.OnDetach();

        private string GetSerializedUniqueName() => this.inheritanceContext?.As<FrameworkElement>()?.DataContext?.GetType().FullName.GetHash(HashAlgorithms.Md5) + "_Listview";

        private async void Initialize()
        {
            if (this.inheritanceContext?.As<FrameworkElement>()?.DataContext != null)
            {
                try
                {
                    this.columnProperties = await Serializer.CreateInstance().DeserializeAsync<ColumnHeaderPropertiesCollection>(this.GetSerializedUniqueName());

                    this.CreateHeaders();

                    if (this.columnProperties == null)
                    {
                        this.columnProperties = new ColumnHeaderPropertiesCollection();
                        return;
                    }

                    this.ApplyPersistent();

                    // at last... Apply the sorting headers
                    if (!string.IsNullOrEmpty(this.columnProperties.SortedHeaderUid))
                        this.AssociatedObject.Columns
                            .FirstOrDefault(x => (x.Header as GridViewColumnHeader).Uid == this.columnProperties.SortedHeaderUid)
                            .IsNotNull(x => this.SetSorting(x.Header as GridViewColumnHeader, this.columnProperties.SortingDirection));
                }
                catch
                {
                    // If this ever happens then because of any IO stuff or the file itself is corrupt...
                }
            }
        }

        private void SetSorting(GridViewColumnHeader columnHeader, ListSortDirection? sortingDirection = null)
        {
            if (!this.IsSortable)
                return;

            var path = GridViewColumnProperties.GetSortingPropertyName(columnHeader);

            if (path == null)
                return;

            var gridView = columnHeader.Column.GetInheritanceContext() as GridView;

            if (gridView == null)
                return;

            var listView = gridView.GetInheritanceContext() as ListView;

            if (listView == null)
                return;

            if (this.sortedHeader != null)
            {
                AdornerLayer.GetAdornerLayer(this.sortedHeader).Remove(this.sortAdorner);
                listView.Items.SortDescriptions.Clear();
            }

            var newSorting = sortingDirection.HasValue ? sortingDirection.Value : ListSortDirection.Ascending;
            if (!sortingDirection.HasValue && this.sortedHeader == columnHeader && this.sortAdorner.Direction == newSorting)
                newSorting = ListSortDirection.Descending;

            this.sortedHeader = columnHeader;

            this.sortAdorner = new SortAdorner(this.sortedHeader, newSorting);
            this.sortAdorner.Foreground = this.inheritanceContext.As<ListView>()?.Foreground;
            AdornerLayer.GetAdornerLayer(this.sortedHeader).Add(this.sortAdorner);

            try
            {
                listView.Items.SortDescriptions.Add(new SortDescription(path, newSorting));
            }
            catch (Exception e)
            {
                Output.WriteLineError("Unable to sort elements: " + e.Message);
            }
        }

        [DataContract]
        private class ColumnHeaderProperties
        {
            [DataMember]
            public string Uid { get; set; }

            [DataMember]
            public double Width { get; set; }
        }

        [DataContract]
        private class ColumnHeaderPropertiesCollection
        {
            [DataMember]
            public List<ColumnHeaderProperties> Columns { get; set; } = new List<ColumnHeaderProperties>();

            [DataMember]
            public string SortedHeaderUid { get; set; }

            public ListSortDirection SortingDirection
            {
                get { return (ListSortDirection)this.SortingDirectionPrimitiv; }
                set { this.SortingDirectionPrimitiv = (int)value; }
            }

            [DataMember]
            public int SortingDirectionPrimitiv { get; set; }
        }
    }
}