using Cauldron.XAML.Validation.ViewModels;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Cauldron.XAML.Validation
{
    /// <exclude/>
    [DebuggerDisplay("PropertyName = {PropertyName}")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ValidatorGroup : List<ValidatorAttributeBase>
    {
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ValidatorGroup(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ConcurrentStack<string> Error { get; private set; } = new ConcurrentStack<string>();

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string PropertyName { get; private set; }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task ValidateAsync(IValidatableViewModel context, PropertyInfo sender, bool validateAll)
        {
            this.Error.Clear();

            for (int i = 0; i < this.Count; i++)
            {
                var item = this[i];

                // Dont invoke these kind of validators
                if (!ValidationHandler.IgnoreAlwaysValidate && !item.AlwaysValidate && sender == null && !validateAll)
                    continue;

                var error = await item.ValidateAsync(sender, context);

                if (!string.IsNullOrEmpty(error))
                {
                    this.Error.Push(error);
                    if (!ValidationHandler.StopValidationOnError)
                        break;
                }
            }
        }
    }
}