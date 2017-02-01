using System.Collections.ObjectModel;

namespace Cauldron.XAML.Validation
{
    internal sealed class ValidatorCollection : KeyedCollection<string, ValidatorGroup>
    {
        protected override string GetKeyForItem(ValidatorGroup item)
        {
            return item.PropertyName;
        }
    }
}