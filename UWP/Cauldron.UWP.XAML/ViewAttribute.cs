using System;

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
        /// Gets the <see cref="Type"/> of the view
        /// </summary>
        public Type ViewType { get; private set; }
    }
}