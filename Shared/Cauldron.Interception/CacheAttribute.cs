using System;

namespace Cauldron.Interception
{
    /// <summary>
    /// Provides a one time caching for the intercepted method.
    /// <para/>
    /// The cache is dependent to the passed arguments. The arguments requires a proper implementation of <see cref="object.GetHashCode"/> and a unique <see cref="object.ToString"/> value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CacheAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CacheAttribute"/>
        /// </summary>
        public CacheAttribute()
        {
        }
    }
}