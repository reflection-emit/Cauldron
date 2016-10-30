using Windows.UI.Xaml.Controls;

namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// Provides a behaviour that make it possible for <see cref="TextBox.Header"/> templates to access the parent <see cref="TextBox"/> properties
    /// </summary>
    public sealed class TextBoxHeader : Behaviour<TextBox>
    {
        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
            var helper = this.AssociatedObject.Header as HeaderBehaviourHelper;

            if (helper == null)
            {
                helper = new HeaderBehaviourHelper();
                //helper.Content = this.AssociatedObject.Header;
                //helper.Object = this.AssociatedObject;
                this.AssociatedObject.Header = helper;
            }
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
        }
    }
}