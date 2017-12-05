using System;
using System.Windows;
using System.Windows.Controls;

namespace Cauldron.XAML
{
    /// <summary>
    /// Specifies a view for a the viewmodel
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ViewAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ViewAttribute"/>
        /// </summary>
        /// <param name="viewType">The type of view to associate with the viewmodel</param>
        public ViewAttribute(Type viewType)
        {
            this.ViewType = viewType;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ViewAttribute"/>
        /// </summary>
        /// <param name="viewName">
        /// The name of view to associate with the viewmodel.
        /// The name can be the key of a <see cref="DataTemplate"/> or the name of a <see cref="Control"/>.
        /// </param>
        public ViewAttribute(string viewName)
        {
            this.ViewName = viewName;
        }

        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        public string ViewName { get; private set; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the view
        /// </summary>
        public Type ViewType { get; private set; }
    }
}