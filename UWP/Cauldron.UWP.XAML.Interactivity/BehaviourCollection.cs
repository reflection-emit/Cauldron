using Cauldron.Core.Extensions;
using Cauldron.XAML.Interactivity.Attributes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

#if WINDOWS_UWP
using Windows.UI.Xaml;

#else

using System.Windows;

#endif

namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// Represents a collection of <see cref="IBehaviour"/>
    /// </summary>
    public sealed class BehaviourCollection : BehaviourCollection<IBehaviour>
    {
        internal BehaviourCollection(DependencyObject owner) : base(owner)
        {
        }
    }

    /// <summary>
    /// Represents a collection of <see cref="IBehaviour"/>
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IBehaviour"/> implementations in the collection</typeparam>
    public class BehaviourCollection<T> : Collection<T> where T : class, IBehaviour
    {
        private object dataContext;
        private bool isOwnerLoaded;
        private DependencyObject owner;

        internal BehaviourCollection(DependencyObject owner)
        {
            this.owner = owner;
            var frameworkElement = owner as FrameworkElement;

            if (frameworkElement != null)
            {
                frameworkElement.Loaded += FrameworkElement_Loaded;
                frameworkElement.Unloaded += FrameworkElement_Unloaded;
                frameworkElement.DataContextChanged += FrameworkElement_DataContextChanged;
            }
        }

        /// <summary>
        /// Removes all behaviours that was assigned by a template
        /// </summary>
        public void RemoveAllTemplateAssignedBehaviours()
        {
            var items = this.Where(x => x.IsAssignedFromTemplate);

            foreach (var item in items)
            {
                item.Detach();
                this.Remove(item);
            }
        }

        /// <summary>
        /// Inserts an element into the <see cref="BehaviourCollection{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert.The value can be null for reference types.</param>
        protected override void InsertItem(int index, T item)
        {
            var attr = item.GetType().GetTypeInfo().GetCustomAttribute<BehaviourUsageAttribute>();
            item.AssociatedObject = this.owner;

            if (attr == null || (attr != null && attr.AllowMultiple))
                base.InsertItem(index, item);
            else if (!attr.AllowMultiple)
            {
                var type = item.GetType();

                // exclude the behaviour if a subclass of the same behaviour is already in the collection
                if (this.Any(x => x.GetType().GetTypeInfo().IsSubclassOf(type)))
                    return;

                base.InsertItem(index, item);
            }

            if (!(this.owner is FrameworkElement))
            {
                foreach (var o in this.Items)
                {
                    o.Attach();
                    this.SetDataContext();
                }
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="BehaviourCollection{T}"/>
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            this[index].Detach();
            base.RemoveItem(index);
        }

        private void Behaviour_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (var item in this.Items)
                item.DataContextPropertyChanged(e.PropertyName);
        }

#if WINDOWS_UWP
        private void FrameworkElement_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
#else

        private void FrameworkElement_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
#endif
        {
            if (!this.isOwnerLoaded)
                return;

            this.SetDataContext();
        }

        private void FrameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            this.isOwnerLoaded = true;

            foreach (var item in this.Items)
            {
                item.Attach();
                this.SetDataContext();
            }
        }

        private void FrameworkElement_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.Items)
                item.Detach();

            var frameworkElement = this.owner as FrameworkElement;

            if (frameworkElement != null)
            {
                frameworkElement.Loaded -= FrameworkElement_Loaded;
                frameworkElement.Unloaded -= FrameworkElement_Unloaded;
                frameworkElement.DataContextChanged -= FrameworkElement_DataContextChanged;
            }

            if (this.dataContext != null && this.dataContext is INotifyPropertyChanged)
                this.dataContext.As<INotifyPropertyChanged>().PropertyChanged -= Behaviour_PropertyChanged;

            this.Clear();
        }

        private void SetDataContext()
        {
            if (this.dataContext != null && this.dataContext is INotifyPropertyChanged)
                this.dataContext.As<INotifyPropertyChanged>().PropertyChanged -= Behaviour_PropertyChanged;

            this.dataContext = this.owner?.As<FrameworkElement>()?.DataContext;

            if (this.dataContext != null && this.dataContext is INotifyPropertyChanged)
                this.dataContext.As<INotifyPropertyChanged>().PropertyChanged += Behaviour_PropertyChanged;

            foreach (var item in this.Items)
                item.DataContextChanged(this.dataContext);
        }
    }
}