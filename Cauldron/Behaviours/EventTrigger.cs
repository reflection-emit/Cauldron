using System.Windows;
using System.Windows.Markup;

namespace Cauldron.Behaviours
{
    /// <summary>
    /// Provides a Behaviour that can invoke <see cref="ActionBase"/> behaviours.
    /// <para/>
    /// The <see cref="EventTrigger"/> is triggered by an event of the associated <see cref="FrameworkElement"/>
    /// </summary>
    [ContentProperty(nameof(Actions))]
    public sealed partial class EventTrigger : Behaviour<FrameworkElement>
    {
    }
}