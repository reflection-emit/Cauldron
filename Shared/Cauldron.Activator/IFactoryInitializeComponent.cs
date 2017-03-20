namespace Cauldron.Activator
{
    /// <summary>
    /// Represents a component with an explicit instance initializer that will be invoked by the <see cref="Factory"/> after creation
    /// </summary>
    public interface IFactoryInitializeComponent
    {
        /// <summary>
        /// Occures after object creation by the <see cref="Factory"/>.
        /// </summary>
        void OnInitializeComponent();
    }
}