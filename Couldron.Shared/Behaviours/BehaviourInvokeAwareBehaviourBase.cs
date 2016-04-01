using System;

#if NETFX_CORE
using Windows.UI.Xaml;

#else

using System.Windows;

#endif

namespace Couldron.Behaviours
{
    /// <summary>
    /// A base class for behaviour invoke aware behaviours
    /// </summary>
    /// <typeparam name="T">The control type the behaviour can be attached to</typeparam>
    public abstract class BehaviourInvokeAwareBehaviourBase<T> : Behaviour<T> where T : FrameworkElement
    {
        private object oldDataContext;

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
                oldDataContext.CastTo<INotifyBehaviourInvokation>().IsNotNull(x => x.BehaviourInvoke -= BehaviourInvoke);

            if (this.AssociatedObject.DataContext != oldDataContext)
            {
                oldDataContext = this.AssociatedObject.DataContext;
                this.AssociatedObject.DataContext.CastTo<INotifyBehaviourInvokation>().IsNotNull(x => x.BehaviourInvoke += BehaviourInvoke);
            }
        }

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                this.AssociatedObject.DataContext.CastTo<INotifyBehaviourInvokation>().IsNotNull(x => x.BehaviourInvoke -= BehaviourInvoke);
                this.oldDataContext = null;
            }
        }

        private void BehaviourInvoke(object sender, BehaviourInvokationArgs e)
        {
            if (string.Equals(this.Name, e.BehaviourName, StringComparison.OrdinalIgnoreCase))
                this.Invoke();
        }
    }
}