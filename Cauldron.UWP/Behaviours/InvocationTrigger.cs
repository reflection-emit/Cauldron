using Windows.UI.Xaml.Markup;

namespace Cauldron.Behaviours
{
    /// <summary>
    /// Provides a Behaviour that can invoke <see cref="ActionBase"/> behaviours using invoke awareness event
    /// </summary>
    [ContentProperty(Name = nameof(Actions))]
    public partial class InvocationTrigger
    {
    }
}