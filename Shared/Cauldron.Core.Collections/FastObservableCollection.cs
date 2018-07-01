using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Cauldron.Core.Collections
{
    /// <summary>
    /// Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
#if !NETFX_CORE

    [Serializable]
#else

    using System.Runtime.Serialization;

    [DataContract]
#endif

    public class FastObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged, IEquatable<ObservableCollection<T>>, IEquatable<FastObservableCollection<T>>
    {
        private const string IndexerName = "Item[]";
        private Action onChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="FastObservableCollection{T}"/> that is empty and has default initial capacity.
        /// </summary>
        public FastObservableCollection() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FastObservableCollection{T}"/> that is empty and has default initial capacity.
        /// </summary>
        /// <param name="onCollectionChangedAction">The action to perform if the collection has changed</param>
        public FastObservableCollection(Action onCollectionChangedAction) : base() => this.onChanged = onCollectionChangedAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="FastObservableCollection{T}"/> class
        /// that contains elements copied from the specified list
        /// </summary>
        /// <param name="items">The list whose elements are copied to the new list.</param>
        /// <param name="onCollectionChangedAction">The action to perform if the collection has changed</param>
        /// <remarks>
        /// The elements are copied onto the <see cref="FastObservableCollection{T}"/> in the same order they are read by the enumerator of the list.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> list is a null reference </exception>
        public FastObservableCollection(IEnumerable<T> items, Action onCollectionChangedAction) : base(items == null ? new List<T>() : new List<T>(items)) =>
            this.onChanged = onCollectionChangedAction;

        /// <summary>
        /// Occurs when the collection changes, either by adding or removing an item.
        /// </summary>
#if !NETFX_CORE

        [field: NonSerialized]
#endif
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
#if !NETFX_CORE

        [field: NonSerialized]
#endif
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="FastObservableCollection{T}"/>
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be added to the end of the <see cref="FastObservableCollection{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null,
        /// if type T is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is null.
        /// </exception>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            int index = this.Items.Count;
            var list = new List<object>();

            foreach (var item in collection)
            {
                base.InsertItem(index++, item);
                list.Add(item);
            }

            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(IndexerName);

            OnCollectionChanged(NotifyCollectionChangedAction.Add, list);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="FastObservableCollection{T}"/>
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be added to the end of the <see cref="FastObservableCollection{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null,
        /// if type T is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// collection is null.
        /// </exception>
        public void AddRange(IEnumerable collection) => this.AddRange(collection.Cast<T>());

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is ObservableCollection<T>)
                return this.Equals(obj as ObservableCollection<T>);

            if (obj is FastObservableCollection<T>)
                return this.Equals(obj as FastObservableCollection<T>);

            return base.Equals(obj);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(ObservableCollection<T> other)
        {
            if (object.Equals(other, null))
                return false;

            return other.SequenceEqual(this);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(FastObservableCollection<T> other)
        {
            if (object.Equals(other, null))
                return false;

            return other.SequenceEqual(this);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            if (this.Count == 0)
                return base.GetHashCode();

            int result = 0;
            for (int i = 0; i < this.Count; i++)
                result ^= this[i].GetHashCode();

            return result;
        }

        /// <summary>
        /// Move item at <paramref name="oldIndex"/> to <paramref name="newIndex"/>.
        /// </summary>
        public void Move(int oldIndex, int newIndex) => MoveItem(oldIndex, newIndex);

        /// <summary>
        /// Move item at <paramref name="a"/> to <paramref name="b"/>
        /// </summary>
        /// <param name="a">The element to be moved</param>
        /// <param name="b">The element position where a should be moved to</param>
        public void Move(T a, T b) => this.MoveItem(this.IndexOf(a), this.IndexOf(b));

        /// <summary>
        /// Removes a collection of elements from the <see cref="FastObservableCollection{T}"/>.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be removed from the <see cref="FastObservableCollection{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null,
        /// if type T is a reference type.</param>
        /// <exception cref="ArgumentNullException">
        /// collection is null.
        /// </exception>
        public void RemoveRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var leftItems = this.Items.Except(collection).ToArray();
            base.ClearItems();

            int index = this.Items.Count;

            foreach (var item in leftItems)
                base.InsertItem(index++, item);

            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(IndexerName);

            OnCollectionChanged(NotifyCollectionChangedAction.Remove, collection.ToList());
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="FastObservableCollection{T}"/>
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be removed from the <see cref="FastObservableCollection{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null,
        /// if item is a reference type.</param>
        /// <exception cref="ArgumentNullException">collection is null.</exception>
        public void RemoveRange(IEnumerable collection) => this.RemoveRange(collection.Cast<T>());

        /// <exclude/>
        protected override void ClearItems()
        {
            base.ClearItems();
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(IndexerName);
            OnCollectionReset();
        }

        /// <exclude/>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        /// <exclude/>
        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            T removedItem = this[oldIndex];

            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, removedItem);

            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex);
        }

        /// <exclude/>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
            this.onChanged?.Invoke();
        }

        /// <summary>
        /// Raises a PropertyChanged event.
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => this.PropertyChanged?.Invoke(this, e);

        /// <exclude/>
        protected override void RemoveItem(int index)
        {
            T removedItem = this[index];

            base.RemoveItem(index);

            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, index);
        }

        /// <exclude/>
        protected override void SetItem(int index, T item)
        {
            T originalItem = this[index];
            base.SetItem(index, item);

            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, item, index);
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));

        private void OnCollectionChanged(NotifyCollectionChangedAction action, IList changedItems)
        {
            var startingIndex = this.Items.Count - changedItems.Count;

            if (startingIndex < 0)
                OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(action, changedItems));
            else
                OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(action, changedItems, startingIndex));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));

        private void OnCollectionChangedMultiItem(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handlers = this.CollectionChanged;

            if (handlers != null)
            {
                foreach (NotifyCollectionChangedEventHandler handler in handlers.GetInvocationList())
                {
                    /*
                        This is not very speed relevant, because the handlers are not that many.
                        By using reflection here it spares us from referencing to the PresentationFramework.dll
                     */

                    var targetType = handler.Target?.GetType();

                    if (targetType == null || targetType.Name != "CollectionView")
                    {
                        handler(this, e);
                        continue;
                    }

                    var refreshMethod = targetType.GetMethod("Refresh", BindingFlags.Public | BindingFlags.Instance);

                    if (refreshMethod == null)
                    {
                        handler(this, e);
                        continue;
                    }

                    refreshMethod.Invoke(handler.Target, null);
                }
            }

            this.onChanged?.Invoke();
        }

        private void OnCollectionReset() => OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        private void OnPropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }
}