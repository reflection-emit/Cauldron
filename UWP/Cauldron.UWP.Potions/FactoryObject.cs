using Cauldron.Activator;

namespace Cauldron.Potions
{
    /// <summary>
    /// Represents a component instance creator
    /// </summary>
    /// <typeparam name="TInterface">The interface used</typeparam>
    public abstract class FactoryObject<TInterface>
    {
        /// <summary>
        /// Creates a new instance of the object using <see cref="Factory.Create(string, object[])"/>.
        /// </summary>
        /// <returns>A new instance of the object</returns>
        public static TInterface CreateInstance() => Factory.Create<TInterface>();
    }
}