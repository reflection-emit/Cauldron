using Cauldron.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cauldron.Consoles
{
    /// <summary>
    /// Provides extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the property names of all active parameters
        /// </summary>
        /// <param name="executionGroup">The execution group</param>
        /// <returns>A list of property names of all active parameters</returns>
        public static IList<string> GetActiveParameters(this IExecutionGroup executionGroup) =>
            executionGroup
                .GetType()
                .GetPropertiesEx(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                .Select(x => new { Atribute = x.GetCustomAttribute<ParameterAttribute>(), Property = x })
                .Where(x => x.Atribute != null && x.Atribute.activated)
                .Select(x => x.Property.Name)
                .ToList();
    }
}