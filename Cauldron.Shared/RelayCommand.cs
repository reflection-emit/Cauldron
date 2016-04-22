using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Cauldron
{
    /// <summary>
    /// Implements the <see cref="IRelayCommand"/> interface
    /// </summary>
    public class RelayCommand : IRelayCommand
    {
        /// <summary>
        /// Backing field for IsEnabled Property
        /// </summary>
        private bool _IsEnabled;

        private Action action;
        private Func<bool> canexecute;

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand"/> class
        /// </summary>
        /// <param name="action">The action that is invoked on command execution</param>
        /// <param name="canexecute">A delegate that indicates if the command can be executed or not. Should return true if the command can be executed.</param>
        public RelayCommand(Action action, Func<bool> canexecute)
        {
            this.action = action;
            this.canexecute = canexecute;

            this.OnCanExecuteChanged();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand"/> class
        /// </summary>
        /// <param name="action">The action that is invoked on command execution</param>
        public RelayCommand(Action action)
        {
            this.action = action;
            this.canexecute = () => true;
            this.OnCanExecuteChanged();
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value that indicates if the assiociated control is disabled or enabled
        /// </summary>
        public bool IsEnabled
        {
            get { return this._IsEnabled; }
            private set
            {
                if (this._IsEnabled == value)
                    return;

                this._IsEnabled = value;

                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
            }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            this.IsEnabled = this.canexecute();
            return this._IsEnabled;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            this.action();
        }

        /// <summary>
        /// Triggers the <see cref="ICommand.CanExecuteChanged"/> event and forces the control to refresh the execution state
        /// </summary>
        public void RefreshCanExecute()
        {
            this.OnCanExecuteChanged();
        }

        /// <summary>
        /// Occures if the <see cref="RelayCommand.RefreshCanExecute"/> method has been invoked
        /// </summary>
        protected void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Implements the <see cref="IRelayCommand"/> interface
    /// <para/>
    /// <see cref="RelayCommand{T}"/> will pass the <see cref="EventArgs"/> from the control's event to the action delegate
    /// </summary>
    public class RelayCommand<T> : IRelayCommand
    {
        /// <summary>
        /// Backing field for IsEnabled Property
        /// </summary>
        private bool _IsEnabled;

        private Action<T> action;
        private Predicate<T> canexecute;

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand{T}"/> class
        /// </summary>
        /// <param name="action">The action that is invoked on command execution</param>
        /// <param name="canexecute">A delegate that indicates if the command can be executed or not. Should return true if the command can be executed.</param>
        public RelayCommand(Action<T> action, Predicate<T> canexecute)
        {
            this.action = action;
            this.canexecute = canexecute;

            this.OnCanExecuteChanged();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand{T}"/> class
        /// </summary>
        /// <param name="action">The action that is invoked on command execution</param>
        public RelayCommand(Action<T> action)
        {
            this.action = action;
            this.canexecute = x => true;

            this.OnCanExecuteChanged();
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value that indicates if the assiociated control is disabled or enabled
        /// </summary>
        public bool IsEnabled
        {
            get { return this._IsEnabled; }
            set
            {
                if (this._IsEnabled == value)
                {
                    return;
                }

                this._IsEnabled = value;

                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
            }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            this.IsEnabled = this.canexecute((T)parameter);
            return this._IsEnabled;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            this.action((T)parameter);
        }

        /// <summary>
        /// Triggers the <see cref="ICommand.CanExecuteChanged"/> event and forces the control to refresh the execution state
        /// </summary>
        public void RefreshCanExecute()
        {
            this.OnCanExecuteChanged();
        }

        /// <summary>
        /// Occures if the <see cref="RelayCommand.RefreshCanExecute"/> method has been invoked
        /// </summary>
        protected void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}