using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Cauldron.Behaviours
{
    public sealed class TextBoxInputFilter : Behaviour<TextBox>
    {
        #region Dependency Property RegexFilter

        /// <summary>
        /// Identifies the <see cref="RegexFilter" /> dependency property
        /// </summary>
        public static readonly DependencyProperty RegexFilterProperty = DependencyProperty.Register(nameof(RegexFilter), typeof(string), typeof(TextBoxInputFilter), new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets the <see cref="RegexFilter" /> Property
        /// </summary>
        public string RegexFilter
        {
            get { return (string)this.GetValue(RegexFilterProperty); }
            set { this.SetValue(RegexFilterProperty, value); }
        }

        #endregion Dependency Property RegexFilter

        protected override void OnAttach()
        {
            this.AssociatedObject.PreviewTextInput += AssociatedObject_PreviewTextInput;
        }

        protected override void OnDetach()
        {
            this.AssociatedObject.PreviewTextInput -= AssociatedObject_PreviewTextInput;
        }

        private void AssociatedObject_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex(this.RegexFilter);
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}