using System;
using System.Windows;

namespace Couldron
{
    /// <summary>
    /// Provides usefull extension methods extending <see cref="FrameworkElement"/>
    /// </summary>
    public static partial class ExtensionsFrameworkElement
    {
        /// <summary>
        /// Returns the parent object with the specified type of the specified object by processing the logical tree.
        /// </summary>
        /// <param name="element">The object to find the parent object for. This is expected to be either a <see cref="FrameworkElement"/> or a <see cref="FrameworkContentElement"/>.</param>
        /// <param name="dependencyObjectType">The type of the parent to find</param>
        /// <returns>The requested parent object.</returns>
        public static DependencyObject FindLogicalParent(this DependencyObject element, Type dependencyObjectType)
        {
            if (element == null)
                return null;

            var parent = LogicalTreeHelper.GetParent(element);

            if (parent == null)
                return null;
            if (parent.GetType() == dependencyObjectType)
                return parent;
            else
                return FindLogicalParent(parent, dependencyObjectType);
        }

        /// <summary>
        /// Returns the parent object with the specified type of the specified object by processing the logical tree.
        /// </summary>
        /// <typeparam name="T">The type of the parent to find</typeparam>
        /// <param name="element">The object to find the parent object for. This is expected to be either a <see cref="FrameworkElement"/> or a <see cref="FrameworkContentElement"/>.</param>
        /// <returns>The requested parent object.</returns>
        public static T FindLogicalParent<T>(this DependencyObject element) where T : DependencyObject
        {
            return element.FindLogicalParent(typeof(T)) as T;
        }

        /// <summary>
        /// Returns the parent object of the specified object by processing the logical tree.
        /// </summary>
        /// <param name="element">The object to find the parent object for. This is expected to be either a <see cref="FrameworkElement"/> or a <see cref="FrameworkContentElement"/>.</param>
        /// <returns>The requested parent object.</returns>
        public static DependencyObject GetLogicalParent(this DependencyObject element)
        {
            return LogicalTreeHelper.GetParent(element);
        }
    }
}