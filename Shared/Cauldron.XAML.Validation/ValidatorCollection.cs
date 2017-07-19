using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Cauldron.XAML.Validation
{
    [DebuggerDisplay("Count = {Count}")]
    internal sealed class ValidatorCollection : KeyedCollection<string, ValidatorGroup>
    {
        protected override string GetKeyForItem(ValidatorGroup item) => item.PropertyName;
    }
}