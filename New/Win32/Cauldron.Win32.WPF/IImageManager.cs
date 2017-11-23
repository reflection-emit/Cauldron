using System;
using System.Threading.Tasks;

#if WINDOWS_UWP

using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;

#else

using System.Windows.Media.Imaging;
using System.IO;

#endif

namespace Cauldron.XAML
{
    /// <summary>
    /// Represents a type used to retrieving and caching of embedded images
    /// </summary>
    public interface IImageManager : IDisposable
    {
        /// <summary>
        /// Returns an embedded image
        /// <para/>
        /// Returns null if the <paramref name="resourceInfoName"/> was not found
        /// </summary>
        /// <param name="resourceInfoName">The end of the fully qualified name of the embedded resource</param>
        /// <returns>The embedded image</returns>
        Task<BitmapImage> GetImageAsync(string resourceInfoName);

#if WINDOWS_UWP

        /// <summary>
        /// Loads an image from a file
        /// </summary>
        /// <param name="file">The file that contains the image</param>
        /// <returns>The image represented by <see cref="BitmapImage"/></returns>
        Task<BitmapImage> LoadBitmapImageAsync(StorageFile file);

#else

        /// <summary>
        /// Loads an image from a file
        /// </summary>
        /// <param name="file">The file that contains the image</param>
        /// <returns>The image represented by <see cref="BitmapImage"/></returns>
        Task<BitmapImage> LoadBitmapImageAsync(FileInfo file);

#endif
    }
}