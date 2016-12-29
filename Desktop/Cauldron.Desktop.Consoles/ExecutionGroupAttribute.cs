using Cauldron.Localization;
using System;

namespace Cauldron.Consoles
{
    /// <summary>
    /// Specifies additional meta data to the execution group class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ExecutionGroupAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionGroupAttribute"/> class
        /// </summary>
        /// <param name="groupName">
        /// The name of the group.
        /// <para/>
        /// If an implementation of <see cref="ILocalizationSource"/> exist, the parser will use <paramref name="groupName"/> as a key for <see cref="Locale"/>
        /// </param>
        /// <param name="usageExample">An example of the usage of this execution group</param>
        /// <param name="groupIndex">The group index indicates the execution priority of the groups. Higher values means lower priority.</param>
        public ExecutionGroupAttribute(string groupName, string usageExample, uint groupIndex = 0)
        {
            this.GroupName = groupName;
            this.GroupIndex = groupIndex;
            this.UsageExample = usageExample;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionGroupAttribute"/> class
        /// </summary>
        /// <param name="groupName">
        /// The name of the group.
        /// <para/>
        /// If an implementation of <see cref="ILocalizationSource"/> exist, the parser will use <paramref name="groupName"/> as a key for <see cref="Locale"/>
        /// </param>
        /// <param name="groupIndex">The group index indicates the execution priority of the groups. Higher values means lower priority.</param>
        public ExecutionGroupAttribute(string groupName, uint groupIndex = 0) : this(groupName, null, groupIndex)
        {
        }

        /// <summary>
        /// Gets the priority index of the group
        /// </summary>
        public uint GroupIndex { get; private set; }

        /// <summary>
        /// Gets the group name
        /// </summary>
        public string GroupName { get; private set; }

        /// <summary>
        /// Gets the usage example of the group
        /// </summary>
        public string UsageExample { get; private set; }
    }
}