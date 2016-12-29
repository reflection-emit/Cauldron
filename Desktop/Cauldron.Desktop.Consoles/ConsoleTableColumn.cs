using System;
using System.Collections.Generic;

namespace Cauldron.Consoles
{
    /// <summary>
    /// Reprents a column of the table and the corresponding values
    /// </summary>
    public sealed class ConsoleTableColumn
    {
        internal string[][] _text;
        internal int _width;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleTableColumn"/> class
        /// </summary>
        /// <param name="values">A collection of values</param>
        public ConsoleTableColumn(IEnumerable<string> values) : this()
        {
            this.Values.AddRange(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleTableColumn"/> class
        /// </summary>
        /// <param name="values">A collection of values</param>
        public ConsoleTableColumn(params string[] values) : this()
        {
            this.Values.AddRange(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleTableColumn"/> class
        /// </summary>
        public ConsoleTableColumn()
        {
            this.Background = Console.BackgroundColor;
            this.Foreground = Console.ForegroundColor;
            this.AlternativeForeground = ConsoleColor.White;
        }

        /// <summary>
        /// Gets or sets the alignment of column values
        /// </summary>
        public ColumnAlignment Alignment { get; set; }

        /// <summary>
        /// Gets or sets the alternative foreground color for the column values
        /// </summary>
        public ConsoleColor AlternativeForeground { get; set; }

        /// <summary>
        /// Gets or sets the background color of the column values
        /// </summary>
        public ConsoleColor Background { get; set; }

        /// <summary>
        /// Gets or sets the filler character for the space between the columns
        /// </summary>
        public char Filler { get; set; } = ' ';

        /// <summary>
        /// Gets or sets the foreground of the column values
        /// </summary>
        public ConsoleColor Foreground { get; set; }

        /// <summary>
        /// Gets or sets the values of the column
        /// </summary>
        public List<string> Values { get; private set; } = new List<string>();

        /// <summary>
        /// Gets or sets a ratio of the column width depending to the <see cref="Console.WindowWidth"/> and total amount of columns
        /// </summary>
        public float Width { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating if the column values will be wraped to the next line if it is longer than the column width
        /// </summary>
        public bool WrapWords { get; set; } = true;
    }
}