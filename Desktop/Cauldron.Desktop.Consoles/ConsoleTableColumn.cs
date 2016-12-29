using Cauldron.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Consoles
{
    public enum ConsoleTableColumnAlignment
    {
        Left,
        Center,
        Right
    }

    public sealed class ConsoleTableColumn
    {
        internal string[][] text;
        internal int width;

        public ConsoleTableColumn(params string[] values) : this()
        {
            this.Values.AddRange(values);
        }

        public ConsoleTableColumn()
        {
            this.Background = Console.BackgroundColor;
            this.Foreground = Console.ForegroundColor;
        }

        public ConsoleTableColumnAlignment Alignment { get; set; }

        public ConsoleColor Background { get; set; }

        public char Filler { get; set; } = ' ';

        public ConsoleColor Foreground { get; set; }

        public List<string> Values { get; private set; } = new List<string>();

        public bool WrapWords { get; set; } = true;
    }
}