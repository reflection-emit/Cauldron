using System;

namespace Cauldron.Consoles
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ExecutionGroupAttribute : Attribute
    {
        public ExecutionGroupAttribute(string groupName, string usageExample, uint groupIndex = 0)
        {
            this.GroupName = groupName;
            this.GroupIndex = groupIndex;
            this.UsageExample = usageExample;
        }

        public uint GroupIndex { get; private set; }
        public string GroupName { get; private set; }
        public string UsageExample { get; private set; }
    }
}