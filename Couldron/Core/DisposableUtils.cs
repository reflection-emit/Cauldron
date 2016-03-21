using System.Reflection;

namespace Couldron.Core
{
    /// <summary>
    /// Provides utilities for disposing objects
    /// </summary>
    public static class DisposableUtils
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="context">The object to dispose</param>
        public static void DisposeObjects(object context)
        {
            foreach (var property in context.GetType().GetProperties())
            {
                var dontDispose = property.GetCustomAttribute<IgnoreDisposeAttribute>() != null;

                if (dontDispose)
                    continue;

                property.GetValue(context).DisposeAll();
            }
        }
    }
}