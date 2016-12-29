using System;
using System.Collections.Generic;

namespace Cauldron.Consoles
{
    public enum ColumnAlignment
    {
        Left,
        Center,
        Right
    }

    public sealed class ConsoleTableColumn
    {
        internal string[][] _text;
        internal int _width;

        public ConsoleTableColumn(IEnumerable<string> values) : this()
        {
            this.Values.AddRange(values);
        }

        public ConsoleTableColumn(params string[] values) : this()
        {
            this.Values.AddRange(values);
        }

        public ConsoleTableColumn()
        {
            this.Background = Console.BackgroundColor;
            this.Foreground = Console.ForegroundColor;
            this.AlternativeForeground = ConsoleColor.White;
        }

        public ColumnAlignment Alignment { get; set; }

        public ConsoleColor AlternativeForeground { get; set; }

        public ConsoleColor Background { get; set; }

        public char Filler { get; set; } = ' ';

        public ConsoleColor Foreground { get; set; }
        public List<string> Values { get; private set; } = new List<string>();

        /// <summary>
        /// Gets or sets a ratio of the column width depending to the <see cref="Console.WindowWidth"/> and total amount of columns
        /// </summary>
        public float Width { get; set; } = 1;

        public bool WrapWords { get; set; } = true;
    }
}