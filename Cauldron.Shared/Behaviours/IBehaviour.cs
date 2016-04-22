using System;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

#else

using System.Windows;
using System.Windows.Data;

#endif

namespace Cauldron.Behaviours
{
    /// <summary>
    /// Represents a behaviour
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBehaviour<T> : IBehaviour
    {
        /// <summary>
        /// Gets the <see cref="DependencyObject"/> to which the behavior is attached.
        /// </summary>
        T AssociatedObject { get; set; }
    }

    /// <summary>
    /// Represents a behaviour
    /// </summary>
    public interface IBehaviour : IDisposable
    {
        /// <summary>
        /// Gets a value that indicates the behaviour was assigned from a template
        /// </summary>
        bool IsAssignedFromTemplate { get; }

        /// <summary>
        /// Gets or sets a name of the behaviour
        /// </summary>
        string Name { get; set; }

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
    }
}