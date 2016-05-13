using System.Windows.Markup;

namespace Cauldron.Behaviours
{
    /// <summary>
    /// Provides a Behaviour that can invoke <see cref="ActionBase"/> behaviours using invoke awareness event
    /// </summary>
    [ContentProperty(nameof(Actions))]
    public partial class InvocationTrigger
    {
    }
}