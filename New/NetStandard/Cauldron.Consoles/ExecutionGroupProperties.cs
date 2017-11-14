using System.Collections.Generic;

namespace Cauldron.Consoles
{
    internal sealed class ExecutionGroupProperties
    {
        public ExecutionGroupAttribute Attribute { get; set; }
        public IExecutionGroup ExecutionGroup { get; set; }

        public List<ExecutionGroupParameter> Parameters { get; set; }
    }
}