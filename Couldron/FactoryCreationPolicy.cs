namespace Couldron
{
    /// <summary>
    /// Describes the creation policy of an object through the <see cref="Factory"/>
    /// </summary>
    public enum FactoryCreationPolicy
    {
        /// <summary>
        /// Always creates a new instance.
        /// <para/>
        /// Instances are not managed by the <see cref="Factory"/>.
        /// <para/>
        /// Default policy
        /// </summary>
        Instanced,

        /// <summary>
        /// Only a single instance is created and reused everytime.
        /// <para/>
        /// Instances are managed by the <see cref="Factory"/>
        /// </summary>
        Singleton
    }
}