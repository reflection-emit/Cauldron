using System.Windows.Input;

#if NETFX_CORE
using Windows.UI.Xaml;
#else

using System.Windows;

#endif

namespace Couldron.Behaviours
{
    /// <summary>
    /// Provides a behaviour that can invoke a command when the Enter key is pressed
    /// </summary>
    public sealed partial class EnterKeyToCommand
    {
        #region Dependency Property Command

        /// <summary>
        /// Identifies the <see cref="Command" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(EnterKeyToCommand), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Command" /> Property
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(MyPropertyProperty); }
            set { this.SetValue(MyPropertyProperty, value); }
        }

        #endregion Dependency Property Command

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
            this.AssociatedObject.KeyUp += AssociatedObject_KeyUp;
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
            this.AssociatedObject.KeyUp -= AssociatedObject_KeyUp;
        }
    }
}