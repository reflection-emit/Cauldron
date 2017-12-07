using Cauldron.Core.Extensions;
using System;
using System.Reflection;
using System.Windows;

namespace Cauldron.XAML
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Returns the InheritanceContext of the current <see cref="DependencyObject"/>
        /// </summary>
        /// <param name="dependencyObject">The dependency object to get the InheritanceContext from</param>
        /// <returns>The InheritanceContext of the given dependency object</returns>
        public static DependencyObject GetInheritanceContext(this DependencyObject dependencyObject)
        {
            var contextProperty = dependencyObject.GetType().GetPropertyEx("InheritanceContext", BindingFlags.Instance | BindingFlags.NonPublic);

            if (contextProperty == null)
                throw new NullReferenceException("Unable to find the property InheritanceContext in the InputBinding object");

            return contextProperty.GetValue(dependencyObject) as DependencyObject;
        }

        /// <summary>
        /// Gets the window handle for a Windows Presentation Foundation (WPF) window
        /// </summary>
        /// <param name="window">A WPF window object.</param>
        /// <returns>The Windows Presentation Foundation (WPF) window handle (HWND).</returns>
        public static IntPtr GetWindowHandle(this Window window) => new System.Windows.Interop.WindowInteropHelper(window).Handle;
    }
}