using Cauldron.Consoles;
using System;

namespace SampleConsoleApplication
{
    [ExecutionGroup("mainGroup", "-n My Name -N 73")]
    public sealed class MainExecutionGroup : IExecutionGroup
    {
        [Parameter("help-help", "help", "h", "")]
        public bool Help { get; private set; }

        [Parameter("name-help", "name", "n")]
        public string Name { get; private set; }

        [Parameter("number-help", "number", "N")]
        public int? Number { get; private set; }

        [Parameter("path-help", "p")]
        public string Path { get; private set; }

        public void Execute(ParameterParser parser)
        {
            if (Help)
                parser.ShowHelp();
            else
                Console.WriteLine(this.Path);
        }
    }
}