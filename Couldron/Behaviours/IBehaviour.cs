using System;
using System.Windows;
using System.Windows.Data;

namespace Couldron.Behaviours
{
    public interface IBehaviour<T> : IBehaviour
    {
        /// <summary>
        /// Gets the <see cref="DependencyObject"/> to which the behavior is attached.
        /// </summary>
        T AssociatedObject { get; set; }
    }

    public interface IBehaviour : IDisposable
    {
        /// <summary>
        /// Gets a value that indicates the behaviour was assigned from a template
        /// </summary>
        bool IsAssignedFromTemplate { get; }

        /// <summary>
        /// Creates a shallow copy of the instance
        /// </summary>
        /// <returns>A copy of the behaviour</returns>
        IBehaviour Copy();

        /// <summary>
        /// Sets the behaviour's associated object
        /// </summary>
        /// <param name="obj">The associated object</param>
        void SetAssociatedObject(object obj);

        /// <summary>
        /// Attach a data Binding to the property
        /// </summary>
        /// <param name="dp">DependencyProperty that represents the property</param>
        /// <param name="binding">The binding to attach</param>
        BindingExpressionBase SetBinding(DependencyProperty dp, BindingBase binding);
    }
}