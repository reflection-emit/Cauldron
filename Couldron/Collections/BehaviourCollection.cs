using Couldron.Behaviours;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Couldron.Collections
{
    /// <summary>
    /// Represents a collection of <see cref="IBehaviour"/>
    /// </summary>
    public sealed class BehaviourCollection : Collection<IBehaviour>, IDisposable
    {
        internal DependencyObject owner;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of <see cref="BehaviourCollection"/>
        /// </summary>
        public BehaviourCollection() : base()
        {
        }

        /// <summary>
        /// Destructors are used to destruct instances of classes.
        /// </summary>
        ~BehaviourCollection()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Occures if the object has been disposed
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets a value indicating if the object has been disposed or not
        /// </summary>
        public bool IsDisposed { get { return this.disposed; } }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Removes all behaviours that was assigned by a template
        /// </summary>
        public void RemoveAllTemplateAssignedBehaviours()
        {
            var items = this.Where(x => x.IsAssignedFromTemplate);

            foreach (var item in items)
                this.Remove(item);
        }

        /// <summary>
        /// Inserts an element into the <see cref="BehaviourCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert.The value can be null for reference types.</param>
        protected override void InsertItem(int index, IBehaviour item)
        {
            var attr = item.GetType().GetTypeInfo().GetCustomAttribute<BehaviourUsageAttribute>();

            if (attr == null || (attr != null && attr.AllowMultiple))
            {
                base.InsertItem(index, item);
                item.SetAssociatedObject(this.owner);
            }
            else if (!attr.AllowMultiple)
            {
                var type = item.GetType();

                // exclude the behaviour if a subclass of the same behaviour is already in the collection
                if (this.Any(x => x.GetType().IsSubclassOf(type)))
                    return;

                base.InsertItem(index, item);
                item.SetAssociatedObject(this.owner);
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="BehaviourCollection"/>
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            this[index].DisposeAll();
            base.RemoveItem(index);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true if managed resources requires disposing</param>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    for (int i = 0; i < this.Count; i++)
                        this[i].Dispose();
                }

                // Note disposing has been done.
                disposed = true;

                if (this.Disposed != null)
                    this.Disposed(this, EventArgs.Empty);
            }
        }
    }
}