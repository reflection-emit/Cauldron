using Cauldron.Activator;
using Cauldron;
using Cauldron.Threading;
using Cauldron.XAML.Navigation;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cauldron.XAML.ViewModels
{
    /// <summary>
    /// Represents the base class of a ViewModel
    /// </summary>
    public abstract class ViewModelBase : DisposableBase, IViewModel
    {
        private IDispatcher _dispatcher;
        private Guid? _id;
        private bool _isLoading = false;
        private IMessageDialog _messageDialog;
        private INavigator _navigator;

        /// <summary>
        /// Initializes a new instance of <see cref="ViewModelBase"/>
        /// </summary>
        public ViewModelBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ViewModelBase"/>
        /// </summary>
        /// <param name="id">A unique identifier of the viewmodel</param>
        public ViewModelBase(Guid id)
        {
            this._id = id;
        }

        /// <summary>
        /// Occures if a behaviour should be invoked
        /// </summary>
        public event EventHandler<BehaviourInvocationArgs> BehaviourInvoke;

        /// <summary>
        /// Occures if the <see cref="IsLoading"/> property has changed.
        /// </summary>
        public event EventHandler IsLoadingChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the <see cref="Dispatcher"/> this <see cref="IDispatcher "/> is associated with.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IDispatcher Dispatcher
        {
            get
            {
                if (this._dispatcher == null)
                    this._dispatcher = Factory.Create<IDispatcher>();

                return this._dispatcher;
            }
        }

        /// <summary>
        /// Gets the unique Id of the view model
        /// </summary>
        public Guid Id
        {
            get
            {
                if (!this._id.HasValue)
                    this._id = Guid.NewGuid();

                return this._id.Value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the viewmodel is loading
        /// </summary>
        public bool IsLoading
        {
            get { return this._isLoading; }
            set
            {
                if (this._isLoading == value)
                    return;

                this._isLoading = value;

                this.OnIsLoadingChanged();
                this.IsLoadingChanged?.Invoke(this, EventArgs.Empty);
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the message dialog
        /// </summary>
        public IMessageDialog MessageDialog
        {
            get
            {
                if (this._messageDialog == null)
                    this._messageDialog = Factory.Create<IMessageDialog>();

                return this._messageDialog;
            }
        }

        /// <summary>
        /// Gets the window navigator
        /// </summary>
        public INavigator Navigator
        {
            get
            {
                if (this._navigator == null)
                    this._navigator = Factory.Create<INavigator>();

                return this._navigator;
            }
        }

        /// <summary>
        /// Centralized error handling
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> that occured</param>
        public virtual void OnException(Exception e)
        {
            throw e;
        }

        /// <summary>
        /// Invokes the <see cref="BehaviourInvoke"/> event
        /// </summary>
        /// <param name="behaviourName">The name of the behaviour to invoke</param>
        public async void RaiseNotifyBehaviourInvoke(string behaviourName) =>
            await this.Dispatcher.RunAsync(() => this.BehaviourInvoke?.Invoke(this, new BehaviourInvocationArgs(behaviourName)));

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        public async void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            if ((propertyName != null && propertyName.EndsWith("Command")) || this.BeforeRaiseNotifyPropertyChanged(propertyName))
                return;

            await this.Dispatcher.RunAsync(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));

            this.AfterRaiseNotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Occures after the event <see cref="PropertyChanged"/> has been invoked
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        protected virtual void AfterRaiseNotifyPropertyChanged(string propertyName)
        {
        }

        /// <summary>
        /// Occured before the <see cref="PropertyChanged"/> event is invoked.
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        /// <returns>Returns true if <see cref="RaisePropertyChanged(string)"/> should be cancelled. Otherwise false</returns>
        protected virtual bool BeforeRaiseNotifyPropertyChanged(string propertyName) => false;

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            if (disposeManaged)
                MessageManager.Unsubscribe(this);
        }

        /// <summary>
        /// Occures when the value of the <see cref="IsLoading"/> property has changed.
        /// </summary>
        protected virtual void OnIsLoadingChanged() { }
    }
}