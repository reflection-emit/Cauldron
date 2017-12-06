using System;
using System.Collections.Generic;

namespace Cauldron.Core
{
    /// <summary>
    /// Defines methods to support the comparison of objects for equality
    /// </summary>
    /// <typeparam name="T">The type of objects to compare</typeparam>
    public sealed class DynamicEqualityComparer<T> : IEqualityComparer<T>
    {
        private Func<T, int> hashCode;
        private Func<T, T, bool> predicate;

        /// <summary>
        /// Initializes a new instance of <see cref="DynamicEqualityComparer{T}"/>
        /// </summary>
        /// <param name="predicate">An expression used to determines whether the specified object are equal</param>
        public DynamicEqualityComparer(Func<T, T, bool> predicate) : this(predicate, x => x.GetHashCode())
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DynamicEqualityComparer{T}"/>
        /// </summary>
        /// <param name="predicate">An expression used to determines whether the specified object are equal</param>
        /// <param name="hashCode">An expression used to determin hash code for the specified object</param>
        public DynamicEqualityComparer(Func<T, T, bool> predicate, Func<T, int> hashCode)
        {
            this.predicate = predicate;
            this.hashCode = hashCode;
        }

        /// <summary>
        /// Determines whether the specified object are equal
        /// </summary>
        /// <param name="x">The first object of type <typeparamref name="T"/> to compare</param>
        /// <param name="y">The second object of type <typeparamref name="T"/> to compare</param>
        /// <returns>True if the specified objects are equal; otherwise, false</returns>
        public bool Equals(T x, T y) => this.predicate(x, y);

        /// <summary>
        /// Returns a hash code for the specified object
        /// </summary>
        /// <param name="obj">The <see cref="object"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object</returns>
        public int GetHashCode(T obj) => this.hashCode(obj);
    }
}