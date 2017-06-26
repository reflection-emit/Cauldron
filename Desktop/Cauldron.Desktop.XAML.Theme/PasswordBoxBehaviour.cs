using Cauldron.Cryptography;
using Cauldron.Localization;
using Cauldron.XAML.Interactivity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cauldron.XAML.Theme
{
    internal sealed class PasswordBoxBehaviour : Behaviour<PasswordBox>
    {
        private System.Windows.Shapes.Rectangle passwordStrengthDisplay;

        protected override void OnAttach()
        {
            this.AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
            this.passwordStrengthDisplay = this.AssociatedObject.FindVisualChildByName("PasswordStrengthDisplay") as System.Windows.Shapes.Rectangle;
        }

        protected override void OnDetach()
        {
            this.passwordStrengthDisplay = null;
            this.AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
        }

        private void AssociatedObject_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.passwordStrengthDisplay == null)
                return;

            var passwordScore = CryptoUtils.GetPasswordScore(this.AssociatedObject.Password);

            switch (passwordScore)
            {
                case PasswordScore.Blank:
                    this.passwordStrengthDisplay.Fill = Application.Current.Resources[CauldronTheme.LightBorderBrush] as Brush;
                    break;

                case PasswordScore.VeryWeak:
                    this.passwordStrengthDisplay.Fill = Brushes.Red;
                    break;

                case PasswordScore.Weak:
                    this.passwordStrengthDisplay.Fill = Brushes.Tomato;
                    break;

                case PasswordScore.Medium:
                    this.passwordStrengthDisplay.Fill = Brushes.Yellow;
                    break;

                case PasswordScore.Strong:
                    this.passwordStrengthDisplay.Fill = Brushes.YellowGreen;
                    break;

                case PasswordScore.VeryStrong:
                    this.passwordStrengthDisplay.Fill = Brushes.Green;
                    break;
            }

            this.passwordStrengthDisplay.ToolTip = Locale.Current[passwordScore.ToString()];
        }
    }
}