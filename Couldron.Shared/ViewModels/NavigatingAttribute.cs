using System;

namespace Couldron.ViewModels
{
    /// <summary>
    /// Specifies the methods that can be invoked on navigation
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class NavigatingAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatingAttribute"/> class
        /// </summary>
        /// <param name="methodNames">The methods that can be invoked on navigation</param>
        /// <exception cref="ArgumentException">Parameter <paramref name="methodNames"/> is null or empty</exception>
        public NavigatingAttribute(params string[] methodNames)
        {
            if (methodNames == null || methodNames.Length == 0)
                throw new ArgumentException("", nameof(methodNames));
            this.MethodNames = methodNames;
        }

        /// <summary>
        /// Gets the method name of the methods that can be invoked on navigation
        /// </summary>
        public string[] MethodNames { get; private set; }
    }
}