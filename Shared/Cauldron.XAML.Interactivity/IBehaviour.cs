namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// Represents a behaviour
    /// </summary>
    public interface IBehaviour
    {
        /// <summary>
        /// Gets the <see cref="DependencyObject"/> to which the behavior is attached.
        /// </summary>
        object AssociatedObject { get; set; }

        /// <summary>
        /// Gets a value that indicates the behaviour was assigned from a template
        /// </summary>
        bool IsAssignedFromTemplate { get; }

        /// <summary>
        /// Gets or sets a name of the behaviour
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Attaches a behaviour to a <see cref="DependencyObject"/>
        /// </summary>
        void Attach();

        /// <summary>
        /// Creates a shallow copy of the instance
        /// </summary>
        /// <returns>A copy of the behaviour</returns>
        IBehaviour Copy();

        /// <summary>
        /// Occures if the data context of the <see cref="AssociatedObject"/> has changed.
        /// This is only valid if <see cref="AssociatedObject"/> is a <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="newDataContext">The new datacontext to assign to</param>
        void DataContextChanged(object newDataContext);

        /// <summary>
        /// Occurs when a property value changes of the <see cref="AssociatedObject"/>.DataContext.
        /// This is only valid if <see cref="AssociatedObject"/> is a <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="name">The name of the property that has changed</param>
        void DataContextPropertyChanged(string name);

        /// <summary>
        /// Detaches a behviour from a <see cref="DependencyObject"/>
        /// </summary>
        void Detach();
    }
}