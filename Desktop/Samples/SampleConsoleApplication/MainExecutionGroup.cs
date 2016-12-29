using Cauldron.Consoles;
using System;

namespace SampleConsoleApplication
{
    [ExecutionGroup("mainGroup", "-n My Name -N 73")]
    public sealed class MainExecutionGroup : IExecutionGroup
    {
        public bool CanExecute { get; set; }

        [Option("help-help", "help", "h", "")]
        public bool Help { get; private set; }

        [Option("name-help", "name", "n")]
        public string Name { get; private set; }

        [Option("number-help", "number", "N")]
        public int? Number { get; private set; }

        [Option("path-help", "p")]
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