using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace Cauldron.Behaviours
{
    /// <summary>
    /// Provides a behaviour that enables binding to the <see cref="PasswordBox"/> password property
    /// </summary>
    [SecurityCritical]
    public sealed class PasswordBoxBinding : Behaviour<PasswordBox>
    {
        #region Dependency Property Password

        /// <summary>
        /// Identifies the <see cref="Password" /> dependency property
        /// </summary>
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(nameof(Password), typeof(SecureString), typeof(PasswordBoxBinding), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Password" /> Property
        /// </summary>
        public SecureString Password
        {
            get { return (SecureString)this.GetValue(PasswordProperty); }
            set { this.SetValue(PasswordProperty, value); }
        }

        #endregion Dependency Property Password

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
            this.AssociatedObject.LostFocus += AssociatedObject_LostFocus;
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
            this.AssociatedObject.LostFocus -= AssociatedObject_LostFocus;
        }

        private void AssociatedObject_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Password = this.AssociatedObject.Password.ToSecureString();
        }
    }
}