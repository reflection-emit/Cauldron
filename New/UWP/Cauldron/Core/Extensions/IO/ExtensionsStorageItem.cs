using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Cauldron
{
    /// <summary>
    /// Provides a set of extensions for <see cref="StorageFile"/>
    /// </summary>
    public static class ExtensionsStorageItem
    {
        /// <summary>
        /// Gets the timestamp of the last time the file was modified.
        /// </summary>
        /// <param name="file">The file</param>
        /// <returns>The timestamp.</returns>
        public static async Task<DateTime> GetDateModifiedAsync(this StorageFile file)
        {
            var basicProperty = await file.GetBasicPropertiesAsync();
            return basicProperty.DateModified.DateTime;
        }
    }
}