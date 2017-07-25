using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.XAML.Validation
{
    /// <exclude/>
    [DebuggerDisplay("Count = {Count}")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ValidatorCollection : KeyedCollection<string, ValidatorGroup>
    {
        internal ValidatorCollection()
        {
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override string GetKeyForItem(ValidatorGroup item) => item.PropertyName;
    }
}