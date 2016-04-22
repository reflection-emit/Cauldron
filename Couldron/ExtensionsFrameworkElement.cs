using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Cauldron
{
    /// <summary>
    /// Provides usefull extension methods extending <see cref="FrameworkElement"/>
    /// </summary>
    public static partial class ExtensionsFrameworkElement
    {
        /// <summary>
        /// Returns all logical childs and sub child (recursively) of the element that matches the given type
        /// </summary>
        /// <typeparam name="T">The typ of child to search for</typeparam>
        /// <param name="element">The parent element</param>
        /// <returns>A collection of <see cref="FrameworkElement"/></returns>
        public static IEnumerable<FrameworkElement> FindLogicalChildren<T>(this DependencyObject element)
        {
            List<FrameworkElement> elements = new List<FrameworkElement>();
            FindLogicalChildren(typeof(T), element as FrameworkElement, elements);
            return elements;
        }

        /// <summary>
        /// Returns all logical childs and sub child (recursively) of the element that matches the given type
        /// </summary>
        /// <param name="element">The parent element</param>
        /// <param name="dependencyObjectType">The typ of child to search for</param>
        /// <returns>A collection of <see cref="FrameworkElement"/></returns>
        public static IEnumerable FindLogicalChildren(this DependencyObject element, Type dependencyObjectType)
        {
            List<FrameworkElement> elements = new List<FrameworkElement>();
            FindLogicalChildren(dependencyObjectType, element as FrameworkElement, elements);
            return elements;
        }

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

        private static void FindLogicalChildren(Type childType, FrameworkElement element, List<FrameworkElement> list)
        {
            if (element != null)
            {
                foreach (var item in LogicalTreeHelper.GetChildren(element))
                {
                    var child = item as FrameworkElement;

                    if (child == null)
                        continue;

                    if (child != null && child.GetType() == childType)
                        list.Add(child);

                    if (child != null)
                        FindVisualChildren(childType, child, list);
                }
            }
        }
    }
}