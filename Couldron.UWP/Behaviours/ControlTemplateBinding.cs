using Windows.UI.Xaml;

namespace Couldron.Behaviours
{
    public sealed partial class ControlTemplateBinding : Behaviour<FrameworkElement>
    {
        #region Dependency Property SourceProperty

        /// <summary>
        /// Identifies the <see cref="SourceProperty" /> dependency property
        /// </summary>
        public static readonly DependencyProperty SourcePropertyProperty = DependencyProperty.Register(nameof(SourceProperty), typeof(string), typeof(ControlTemplateBinding), new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets the <see cref="SourceProperty" /> Property
        /// </summary>
        public string SourceProperty
        {
            get { return (string)this.GetValue(SourcePropertyProperty); }
            set { this.SetValue(SourcePropertyProperty, value); }
        }

        #endregion Dependency Property SourceProperty
    }
}