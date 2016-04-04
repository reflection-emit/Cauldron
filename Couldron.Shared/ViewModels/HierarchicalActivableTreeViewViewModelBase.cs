using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Couldron.ViewModels
{
    /// <summary>
    /// Represents a hierarchical viewmodel base that implements a <see cref="IsActive"/> property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class HierarchicalActivableTreeViewViewModelBase<T> : DisposableViewModelBase where T : HierarchicalActivableTreeViewViewModelBase<T>
    {
        private bool? isActive = false;

        /// <summary>
        /// Initiates a new instance of <see cref="HierarchicalActivableTreeViewViewModelBase{T}"/>
        /// </summary>
        public HierarchicalActivableTreeViewViewModelBase()
        {
            this.Items = new ObservableCollection<T>();
            this.Items.CollectionChanged += Children_CollectionChanged;
        }

        /// <summary>
        /// Gets or sets a value that indicates if the item is active or not
        /// </summary>
        public bool? IsActive
        {
            get { return this.isActive; }
            set
            {
                if (this.isActive == value)
                    return;

                this.isActive = value;

                if (value.HasValue)
                {
                    foreach (var item in this.Items)
                        item.IsActive = value;
                }

                this.OnPropertyChanged();

                this.Parent.IsNotNull(x => x.SetParentIsActive(this.IsActive));
            }
        }

        /// <summary>
        /// Gets a <see cref="ObservableCollection{T}"/> of <typeparamref name="T"/>
        /// </summary>
        public ObservableCollection<T> Items { get; private set; }

        /// <summary>
        /// Gets the parent of the current element
        /// </summary>
        public T Parent { get; internal set; }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in e.NewItems)
                item.CastTo<T>().Parent = this as T;
        }

        private void SetParentIsActive(bool? value)
        {
            if (this.Items.Count(x => x.IsActive.HasValue && x.IsActive.Value) == this.Items.Count)
                this.isActive = true;
            else if (this.Items.Count(x => x.IsActive.HasValue && x.IsActive.Value) > 0)
                this.isActive = null;
            else
                this.isActive = value;

            this.OnPropertyChanged(nameof(IsActive));
            this.Parent.IsNotNull(x => x.SetParentIsActive(this.IsActive));
        }
    }
}