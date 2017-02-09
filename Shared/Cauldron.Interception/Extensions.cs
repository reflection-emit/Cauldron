using System;
using System.ComponentModel;

namespace Cauldron.Interception
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Extensions
    {
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void TryDispose(this object context)
        {
            var disposable = context as IDisposable;
            disposable?.Dispose();
        }
    }
}