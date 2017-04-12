using System;

namespace Cauldron.Activator
{
    /// <summary>
    /// Represents an interface for the <see cref="Factory"/> extension
    /// </summary>
    public interface IFactoryResolver
    {
        /// <summary>
        /// Occures if multiple Types with the same <paramref name="contractName"/> was found.
        /// <para/>
        /// Should return null if <paramref name="ambiguousTypes"/> collection does not contain the required <see cref="Type"/>
        /// </summary>
        /// <param name="ambiguousTypes">A collection of Types that with the same <paramref name="contractName"/></param>
        /// <param name="contractName">The contract name of the implementations</param>
        /// <returns>The selected <see cref="Type"/></returns>
        IFactoryTypeInfo SelectAmbiguousMatch(IFactoryTypeInfo[] ambiguousTypes, string contractName);
    }
}