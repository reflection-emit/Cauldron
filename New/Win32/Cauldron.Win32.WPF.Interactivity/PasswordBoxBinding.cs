using System.Security;
using Cauldron.Cryptography;

#if WINDOWS_UWP

using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

#else

using System.Windows.Controls;
using System.Windows;

#endif

namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// Provides a behaviour that enables binding to the <see cref="PasswordBox"/> password property
    /// </summary>
    public sealed class PasswordBoxBinding : Behaviour<PasswordBox>
    {
        #region Dependency Property Password

        /// <summary>
        /// Identifies the <see cref="Password"/> dependency property
        /// </summary>
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(nameof(Password), typeof(SecureString), typeof(PasswordBoxBinding), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Password"/> Property
        /// </summary>
        public SecureString Password
        {
            get { return this.GetValue(PasswordProperty) as SecureString; }
            set { this.SetValue(PasswordProperty, value); }
        }

        #endregion Dependency Property Password

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
            this.AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
            this.AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
        }

        private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
        {
#if WINDOWS_UWP
            this.Password = this.AssociatedObject.Password.ToSecureString();
#else
            this.Password = this.AssociatedObject.SecurePassword;
#endif
        }
    }
}