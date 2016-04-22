using System.Collections.Generic;

namespace Cauldron.Validation
{
    internal sealed class ValidatorGroup : List<ValidationBaseAttribute>
    {
        public ValidatorGroup(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }
    }
}