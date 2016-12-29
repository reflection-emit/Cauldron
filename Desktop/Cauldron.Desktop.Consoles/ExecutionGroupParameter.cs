using System;
using System.Linq;
using System.Reflection;

namespace Cauldron.Consoles
{
    internal sealed class ExecutionGroupParameter
    {
        public ExecutionGroupParameter(IExecutionGroup executionGroup, PropertyInfo propertyInfo, OptionAttribute attrib)
        {
            this.PropertyInfo = propertyInfo;
            this.ExecutionGroup = executionGroup;
            this.Attribute = attrib;

            this.Parameters = this.PropertyInfo.PropertyType == typeof(bool) ?
                this.Attribute.Parameters.Select(x => string.IsNullOrEmpty(x) ? "" : $"{ParameterParser.ParameterKey}{ParameterParser.ParameterKey}{x}").ToArray() :
                this.Attribute.Parameters.Select(x => string.IsNullOrEmpty(x) ? "" : ParameterParser.ParameterKey + x).ToArray();
        }

        public OptionAttribute Attribute { get; private set; }
        public IExecutionGroup ExecutionGroup { get; set; }
        public string[] Parameters { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
    }
}