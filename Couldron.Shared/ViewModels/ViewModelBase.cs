using Couldron.Core;
using Couldron.Validation;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Couldron.ViewModels
{
    /// <summary>
    /// Represents the Base class of a ViewModel
    /// </summary>
    public abstract class ViewModelBase : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ViewModelBase"/>
        /// </summary>
        [InjectionConstructor]
        public ViewModelBase()
        {
            this.Id = Guid.NewGuid();
            this.Dispatcher = new CouldronDispatcher();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ViewModelBase"/>
        /// </summary>
        /// <param name="id">A unique identifier of the viewmodel</param>
        public ViewModelBase(Guid id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Occures if a behaviour should be invoked
        /// </summary>
        public event EventHandler<BehaviourInvokationArgs> BehaviourInvoke;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the <see cref="Dispatcher"/> this <see cref="CouldronDispatcher "/> is associated with.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced), JsonIgnore]
        public CouldronDispatcher Dispatcher { get; private set; }

        /// <summary>
        /// Gets the unique Id of the view model
        /// </summary>
        [SuppressIsChanged, JsonIgnore]
        public Guid Id { get; private set; }

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        public async void RaiseNotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.OnBeforeRaiseNotifyPropertyChanged(propertyName))
                return;

            if (this.PropertyChanged != null)
                await this.Dispatcher.RunAsync(() => this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));

            this.OnAfterRaiseNotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Occures after the event <see cref="PropertyChanged"/> has been invoked
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        protected virtual void OnAfterRaiseNotifyPropertyChanged(string propertyName)
        {
        }

        /// <summary>
        /// Occured before the <see cref="PropertyChanged"/> event is invoked.
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        /// <returns>Returns true if <see cref="RaiseNotifyPropertyChanged(string)"/> should be cancelled. Otherwise false</returns>
        protected virtual bool OnBeforeRaiseNotifyPropertyChanged(string propertyName)
        {
            return false;
        }

        /// <summary>
        /// Invokes the <see cref="BehaviourInvoke"/> event
        /// </summary>
        /// <param name="behaviourName">The name of the behaviour to invoke</param>
        protected async void RaiseNotifyBehaviourInvoke(string behaviourName)
        {
            if (this.BehaviourInvoke != null)
                await this.Dispatcher.RunAsync(() => this.BehaviourInvoke(this, new BehaviourInvokationArgs(behaviourName)));
        }
    }
}