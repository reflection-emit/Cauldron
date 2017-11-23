using System;
using Cauldron;

#if WINDOWS_UWP

using Windows.UI.Xaml;

#else

using System.Windows;

#endif

namespace Cauldron.XAML.Interactivity.BehaviourInvocation
{
    /// <summary>
    /// A base class for behaviour invoke aware behaviours
    /// </summary>
    /// <typeparam name="T">The control type the behaviour can be attached to</typeparam>
    public abstract class BehaviourInvokeAwareBehaviourBase<T> : Behaviour<T> where T : FrameworkElement
    {
        private object oldDataContext;

        /// <summary>
        /// Gets or sets the method name
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Occures if the behaviour is requested to invoke
        /// </summary>
        protected abstract void Invoke();

        /// <summary>
        /// Occures if the <see cref="FrameworkElement.DataContext"/> of <see cref="Behaviour{T}.AssociatedObject"/> has changed
        /// </summary>
        protected override void OnDataContextChanged()
        {
            if (this.AssociatedObject.DataContext != oldDataContext && oldDataContext != null)
                (oldDataContext as INotifyBehaviourInvocation).IsNotNull(x => x.BehaviourInvoke -= BehaviourInvoke);

            if (this.AssociatedObject.DataContext != oldDataContext)
            {
                oldDataContext = this.AssociatedObject.DataContext;
                (this.AssociatedObject.DataContext as INotifyBehaviourInvocation).IsNotNull(x => x.BehaviourInvoke += BehaviourInvoke);
            }
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
            (this.AssociatedObject.DataContext as INotifyBehaviourInvocation).IsNotNull(x => x.BehaviourInvoke -= BehaviourInvoke);
            this.oldDataContext = null;
        }

        private void BehaviourInvoke(object sender, BehaviourInvocationArgs e)
        {
            if (string.Equals(this.MethodName, e.BehaviourName, StringComparison.OrdinalIgnoreCase))
                this.Invoke();
        }
    }
}