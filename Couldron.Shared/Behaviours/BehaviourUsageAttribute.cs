using System;

namespace Cauldron.Behaviours
{
    /// <summary>
    /// Specifies that a behaviour can only be applied once
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class BehaviourUsageAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BehaviourUsageAttribute"/>
        /// </summary>
        /// <param name="allowMultiple">Disallows multiple application of behavior on the same instance</param>
        public BehaviourUsageAttribute(bool allowMultiple)
        {
            this.AllowMultiple = allowMultiple;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BehaviourUsageAttribute"/>
        /// <para/>
        /// The <see cref="AllowMultiple"/> property is true
        /// </summary>
        public BehaviourUsageAttribute()
        {
            this.AllowMultiple = true;
        }

        /// <summary>
        /// Gets a value that indicates if a behaviour can be applied once or multiple times
        /// </summary>
        public bool AllowMultiple { get; private set; }
    }
}