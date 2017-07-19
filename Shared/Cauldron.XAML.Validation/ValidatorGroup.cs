using Cauldron.XAML.Validation.ViewModels;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Cauldron.XAML.Validation
{
    [DebuggerDisplay("PropertyName = {PropertyName}")]
    internal sealed class ValidatorGroup : List<ValidatorAttributeBase>
    {
        public ValidatorGroup(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        public ConcurrentStack<string> Error { get; private set; } = new ConcurrentStack<string>();
        public string PropertyName { get; private set; }

        public async Task ValidateAsync(IValidatableViewModel context, PropertyInfo sender, bool validateAll)
        {
            this.Error.Clear();

            for (int i = 0; i < this.Count; i++)
            {
                var item = this[i];

                // Dont invoke these kind of validators
                if (!item.AlwaysValidate && sender == null && !validateAll)
                    continue;

                var error = await item.ValidateAsync(sender, context);

                if (!string.IsNullOrEmpty(error))
                {
                    this.Error.Push(error);
                    if (!ValidationHandler.ValidateAll)
                        break;
                }
            }
        }
    }
}