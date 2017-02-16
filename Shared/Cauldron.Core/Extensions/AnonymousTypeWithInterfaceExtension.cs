using System;

namespace Cauldron.Core.Extensions
{
    /// <summary>
    /// Provides usefull extension methods for anonymous class
    /// </summary>
    public static class AnonymousTypeWithInterfaceExtension
    {
        /// <summary>
        /// Creates a new <see cref="Type"/> that implements the properties of an interface defined by <typeparamref name="T"/>
        /// and copies all value of <paramref name="anon"/> to the new object
        /// </summary>
        /// <typeparam name="T">The type of interface to implement</typeparam>
        /// <param name="anon">The anonymous object</param>
        /// <returns>A new object implementing the interface defined by <typeparamref name="T"/></returns>
        public static T CreateObject<T>(this object anon) where T : class
        {
            /* NOTE: This will be implemented by Cauldron.Interception.Fody */
            throw new NotImplementedException("No weaving happend.");
        }
    }
}