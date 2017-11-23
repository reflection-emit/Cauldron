using Cauldron.Core;
using Cauldron.Core.Diagnostics;
using Cauldron.Core.Extensions;
using System;
using System.Reflection;

#if WINDOWS_UWP

using Windows.UI.Xaml;
using System.Runtime.InteropServices.WindowsRuntime;

#else

using System.Windows;
using System.Windows.Markup;

#endif

namespace Cauldron.XAML
{
    /// <summary>
    /// Handles dynamic event registrations
    /// </summary>
    public sealed class DynamicEventHandler : DisposableBase
    {
        private EventInfo eventInfo;
        private Delegate handler;

#if WINDOWS_UWP
        private Action<EventRegistrationToken> remove;
#endif

        /// <summary>
        /// Initializes a new instance of <see cref="DynamicEventHandler"/>
        /// </summary>
        /// <param name="associatedObject">The objects that contains the event</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="eventHandler">A delegate that handles the event</param>
        public DynamicEventHandler(object associatedObject, string eventName, Action<object, object> eventHandler)
        {
            this.EventName = eventName;
            this.AssociatedObject = associatedObject;
            this.EventHandler = eventHandler;

            this.AttachEvent();
        }

        /// <summary>
        /// Gets the <see cref="object"/> to which the event handler is attached.
        /// </summary>
        public object AssociatedObject { get; private set; }

        /// <summary>
        /// Gets the event handler that handles the event
        /// </summary>
        public Action<object, object> EventHandler { get; private set; }

        /// <summary>
        /// Gets the name of the event
        /// </summary>
        public string EventName { get; private set; }

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                this.DetachEvent();
                this.AssociatedObject = null;
                this.EventHandler = null;
            }
        }

#if WINDOWS_UWP

        private EventRegistrationToken AddEventHandler(MethodInfo addMethodInfo, Delegate method)
        {
            var token = addMethodInfo.Invoke(this.AssociatedObject, new object[] { method });

            // in some conditions (I never found out why) the "get" of the event does not return a
            // EventRegistrationToken. In this case we have to use our workaround
            // If someone has an idea why this is happening... please notify me
            if (token == null)
            {
                var eventTokenTable = this.AssociatedObject.GetType().GetTypeInfo().GetDeclaredField("_" + this.EventName);

                // maybe the tokentable is named camelcase
                if (eventTokenTable == null)
                    eventTokenTable = this.AssociatedObject.GetType().GetTypeInfo().GetDeclaredField(this.EventName.LowerFirstCharacter());

                // still no table? maybe camelcased with underscore
                if (eventTokenTable == null)
                    eventTokenTable = this.AssociatedObject.GetType().GetTypeInfo().GetDeclaredField("_" + this.EventName.LowerFirstCharacter());

                // still no table? fuck this... I don't know
                if (eventTokenTable == null)
                    throw new Exception("No token for event found");

                var eventTokenTableInstance = eventTokenTable.GetValue(this.AssociatedObject);
                var getTokenMethod = eventTokenTableInstance.GetType().GetTypeInfo().GetDeclaredMethod("GetToken");
                token = getTokenMethod.Invoke(eventTokenTableInstance, new object[] { method });
            }

            return (EventRegistrationToken)token;
        }

#endif

        private void AttachEvent()
        {
            this.eventInfo = this.AssociatedObject.GetType().GetRuntimeEvent(this.EventName);

            if (this.eventInfo == null)
                this.eventInfo = this.AssociatedObject.GetType().GetEvent(this.EventName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (eventInfo == null)
                return;

            MethodInfo method = null;
#if WINDOWS_UWP

            var addMethodInfo = eventInfo.AddMethod;
            var removeMethodInfo = eventInfo.RemoveMethod;
#endif
            // Get the method that gets invoked if the event is fired
            if (eventInfo.EventHandlerType == typeof(DependencyPropertyChangedEventHandler)) /* Dependency Property event handlers need some special loving */
                method = this.GetType().GetMethod(nameof(ExecuteCommandDependencyPropertyChangedEventHandler),
                    new Type[] { typeof(object), typeof(DependencyPropertyChangedEventArgs) },
                    BindingFlags.Instance | BindingFlags.NonPublic);
            else
                method = this.GetType().GetMethod(nameof(ExecuteCommand),
                    new Type[] { typeof(object), typeof(object) },
                    BindingFlags.Instance | BindingFlags.NonPublic);

            try
            {
                this.handler = method.CreateDelegate(eventInfo.EventHandlerType, this);

#if WINDOWS_UWP

                this.remove = x => removeMethodInfo.Invoke(this.AssociatedObject, new object[] { x });
#endif
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR: The DynamicEventHandler was not able to add a handler to the event: '" + this.EventName + "'");
                Debug.WriteLine("ERROR: Exception was raised: " + e.Message);
                return;
            }

#if WINDOWS_UWP

            WindowsRuntimeMarshal.AddEventHandler(x => this.AddEventHandler(addMethodInfo, x), remove, this.handler);
#else
            if (this.eventInfo.AddMethod.IsPublic)
                this.eventInfo.AddEventHandler(this.AssociatedObject, this.handler);
            else
            {
                var getMethodInfo = this.eventInfo.GetAddMethod(true);
                getMethodInfo.Invoke(this.AssociatedObject, new object[] { this.handler });
            }
#endif
        }

        private void DetachEvent()
        {
#if WINDOWS_UWP
            if (this.remove == null || this.handler == null)
                return;

            WindowsRuntimeMarshal.RemoveEventHandler(this.remove, this.handler);
#else
            if (this.eventInfo == null || this.handler == null)
                return;

            if (this.eventInfo.RemoveMethod.IsPublic)
                this.eventInfo.RemoveEventHandler(this.AssociatedObject, this.handler);
            else
            {
                var removeMethodInfo = this.eventInfo.GetRemoveMethod(true);
                removeMethodInfo.Invoke(this.AssociatedObject, new object[] { this.handler });
            }
#endif
            this.eventInfo = null;
            this.handler = null;
        }

        private void ExecuteCommand(object sender, object args)
        {
            this.EventHandler?.Invoke(sender, args);

            // Get the handled property and set it to true
            args.GetType().GetPropertyEx("Handled").IsNotNull(x => x.SetValue(args, true));
        }

        private void ExecuteCommandDependencyPropertyChangedEventHandler(object sender, DependencyPropertyChangedEventArgs args) =>
            this.ExecuteCommand(sender, args);
    }
}