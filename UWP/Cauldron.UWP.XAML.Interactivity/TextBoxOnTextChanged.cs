using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cauldron.XAML.Interactivity
{
    public sealed class TextBoxOnTextChanged : Behaviour<TextBox>
    {
        protected override void OnAttach()
        {
            this.AssociatedObject.TextChanged += AssociatedObject_TextChanged;
        }

        protected override void OnDetach()
        {
            this.AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        }

        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.Text = this.AssociatedObject.Text;
        }

        #region Dependency Property Text

        /// <summary>
        /// Identifies the <see cref="Text" /> dependency property
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBoxOnTextChanged), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Text" /> Property
        /// </summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        #endregion Dependency Property Text
    }
}