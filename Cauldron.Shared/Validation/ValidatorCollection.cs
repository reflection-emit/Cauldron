using System.Collections.ObjectModel;

namespace Cauldron.Validation
{
    internal sealed class ValidatorCollection : KeyedCollection<string, ValidatorGroup>
    {
        protected override string GetKeyForItem(ValidatorGroup item)
        {
            return item.PropertyName;
        }
    }
}