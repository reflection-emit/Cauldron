using Cauldron.Core;
using System;

namespace Cauldron
{
    /// <summary>
    /// Represents a collection of utility methods
    /// </summary>
    public partial interface IUtils
    {
        /// <summary>
        /// Gets the NetBIOS name of this local computer.
        /// </summary>
        string ComputerName { get; }

        /// <summary>
        /// Get a value that indicates whether any network connection is available.
        /// <para/>
        /// Returns true if a network connection is available, othwise false
        /// </summary>
        bool IsNetworkAvailable { get; }

        /// <summary>
        /// Determines whether <paramref name="a"/> is equal to <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object, object)"/>.
        /// Then it checks all known types with the == operator, then with reflection on 'op_Equality' and as last resort uses <see cref="object.Equals(object, object)"/> to determine equality
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>true if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise, false.</returns>
        bool Equals(object a, object b);

        /// <summary>
        /// Checks the password's strength
        /// </summary>
        /// <param name="password">The password to check</param>
        /// <returns>Returns <see cref="PasswordScore"/> rating</returns>
        PasswordScore GetPasswordScore(string password);

        /// <summary>
        /// Determines whether <paramref name="a"/> is greater than <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object, object)"/>.
        /// Then it checks all known types with the &gt; operator, then with reflection on 'op_GreaterThan'
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>true if <paramref name="a"/> is greater than <paramref name="b"/>; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Greater than operator cannot be applied</exception>
        bool GreaterThan(object a, object b);

        /// <summary>
        /// Determines whether <paramref name="a"/> is greater than or equal to <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object, object)"/>.
        /// Then it checks all known types with the &gt;= operator, then with reflection on 'op_GreaterThanOrEqual'
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>true if <paramref name="a"/> is greater than or equal to <paramref name="b"/>; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Operator cannot be applied</exception>
        bool GreaterThanOrEqual(object a, object b);

        /// <summary>
        /// Determines whether <paramref name="a"/> is less than <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object, object)"/>.
        /// Then it checks all known types with the &lt; operator, then with reflection on 'op_LessThan'
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>true if <paramref name="a"/> is less than <paramref name="b"/>; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Lesser than operator cannot be applied</exception>
        bool LessThan(object a, object b);

        /// <summary>
        /// Determines whether <paramref name="a"/> is less than or equal to <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object, object)"/>.
        /// Then it checks all known types with the &lt;= operator, then with reflection on 'op_LessThanOrEqual'
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>true if <paramref name="a"/> is less than or equal to <paramref name="b"/>; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Operator cannot be applied</exception>
        bool LessThanOrEqual(object a, object b);

        /// <summary>
        /// Determines whether <paramref name="a"/> is unequal to <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object, object)"/>.
        /// Then it checks all known types with the != operator, then with reflection on 'op_Inequality' and as last resort uses <see cref="object.Equals(object, object)"/> to determine unequality
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>true if <paramref name="a"/> is unequal to <paramref name="b"/>; otherwise, false.</returns>
        bool UnEquals(object a, object b);
    }
}