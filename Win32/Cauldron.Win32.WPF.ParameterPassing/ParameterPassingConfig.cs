using System;
using System.Diagnostics;
using System.Windows;

namespace Cauldron.XAML
{
    /// <summary>
    /// Represents the configuration of the parameter passing
    /// </summary>
    public sealed class ParameterPassingConfig
    {
        private Action _ExitDelegate;

        internal ParameterPassingConfig(Process[] processes)
        {
            this.Instances = processes;
            this._ExitDelegate = new Action(() =>
            {
                Environment.Exit(0);
            });

            this.ParameterPassedCallback = new Action<string[]>(args =>
            {
                MessageBox.Show(string.Join("\r\n", args));
            });
        }

        /// <summary>
        /// Gets or sets a value that indicates that the window will be brought to the front of all window on activation. Default value is true.
        /// </summary>
        public bool BringToFront { get; set; } = true;

        /// <summary>
        /// Gets or sets a char that is used as data separator. Default value is space.
        /// </summary>
        public char DataSeparator { get; set; } = ' ';

        /// <summary>
        /// Gets or sets a delegate that is executed to exit the application. The default value is <see cref="Environment.Exit(int)"/> with the value of 0.
        /// </summary>
        public Action ExitDelegate
        {
            get { return this._ExitDelegate; }
            set
            {
                if (value == null)
                    throw new ArgumentException("value cannot be null");

                this._ExitDelegate = value;
            }
        }

        /// <summary>
        /// Gets all instances of the the current process.
        /// </summary>
        public Process[] Instances { get; private set; }

        /// <summary>
        /// Gets or sets a value that indicates if the application can only have one instance per user. The default value is false.
        /// </summary>
        public bool IsSingleInstance { get; set; } = false;

        /// <summary>
        /// Gets or sets a delegate that is executed if a running instance recieves new parameters. This should always be overriden.
        /// </summary>
        public Action<string[]> ParameterPassedCallback { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the parameters are passed to all running instances. The default value is true.
        /// </summary>
        public bool PassToAllInstances { get; set; } = true;

        /// <summary>
        /// Gets or sets a <see cref="Process"/> that is preferred to recieve all passed parameters, if multiple instances of the application is running.
        /// </summary>
        public Process ProcessToPrefer { get; set; } = null;

        /// <summary>
        /// Gets or sets a value that indicates if a random instance is selected or not if multiple instances of the application is running.
        /// </summary>
        public bool RandomSelectInstance { get; set; } = false;

        /// <summary>
        /// Gets or sets a delegate that is used to validate the passed arguments. This is executed before any others.
        /// </summary>
        public Func<string[], bool> ValidationDelegate { get; set; }
    }
}