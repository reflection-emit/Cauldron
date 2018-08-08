using Cauldron.Activator;
using Cauldron;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Linq;
using Cauldron.Reflection;
using Cauldron.Diagnostics;

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
    /// Provides retrieving and caching of embedded images
    /// </summary>
    [Component(typeof(IImageManager), FactoryCreationPolicy.Singleton)]
    public sealed class ImageManager : Factory<IImageManager>, IImageManager, IDisposableObject
    {
        private ConcurrentDictionary<string, BitmapImage> cache = new ConcurrentDictionary<string, BitmapImage>();

        /// <exclude/>
        [ComponentConstructor]
        public ImageManager()
        {
        }

        /// <summary>
        /// Returns an embedded image
        /// <para/>
        /// Returns null if the <paramref name="resourceInfoName"/> was not found
        /// </summary>
        /// <param name="resourceInfoName">The end of the fully qualified name of the embedded resource</param>
        /// <returns>The embedded image</returns>
        public async Task<BitmapImage> GetImageAsync(string resourceInfoName)
        {
            var assemblyAndResourceInfo = Assemblies.AssemblyAndResourceNamesInfo.FirstOrDefault(x => x.Filename.EndsWith(resourceInfoName, StringComparison.OrdinalIgnoreCase));

            if (assemblyAndResourceInfo == null)
                return null;

            if (cache.ContainsKey(assemblyAndResourceInfo.Filename))
                return cache[assemblyAndResourceInfo.Filename];

            var result = await assemblyAndResourceInfo.Assembly.GetManifestResourceStream(assemblyAndResourceInfo.Filename).ToBitmapImageAsync();
            cache.TryAdd(assemblyAndResourceInfo.Filename, result);

            return result;
        }

#if WINDOWS_UWP

        /// <summary>
        /// Loads an image from a file
        /// </summary>
        /// <param name="file">The file that contains the image</param>
        /// <returns>The image represented by <see cref="BitmapImage"/></returns>
        public async Task<BitmapImage> LoadBitmapImageAsync(StorageFile file)
        {
            var key = $"{file.Path}{file.Name}";
            if (this.cache.ContainsKey(key))
                return cache[key];

            try
            {
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(await file.OpenReadAsync());

                if (this.cache.ContainsKey(key))
                    cache.TryAdd(key, bitmapImage);

                return bitmapImage;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }

#else

        /// <summary>
        /// Loads an image from a file
        /// </summary>
        /// <param name="file">The file that contains the image</param>
        /// <returns>The image represented by <see cref="BitmapImage"/></returns>
        public Task<BitmapImage> LoadBitmapImageAsync(FileInfo file)
        {
            var key = $"{file.FullName}";
            if (this.cache.ContainsKey(key))
                return Task.FromResult(cache[key]);

            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.StreamSource = file.OpenRead();
                bitmapImage.Freeze();

                if (this.cache.ContainsKey(key))
                    cache.TryAdd(key, bitmapImage);

                return Task.FromResult(bitmapImage);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }

#endif

        #region IDisposable

        private bool disposed = false;

        /// <summary>
        /// Destructors are used to destruct instances of classes.
        /// </summary>
        ~ImageManager()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Occures if the object has been disposed
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets a value indicating if the object has been disposed or not
        /// </summary>
        public bool IsDisposed { get { return this.disposed; } }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true if managed resources requires disposing</param>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed and unmanaged resources.
                if (disposing)
                {
                    this.cache.Clear();
                }

                // Note disposing has been done.
                disposed = true;

                if (this.Disposed != null)
                    this.Disposed(this, EventArgs.Empty);
            }
        }

        #endregion IDisposable
    }
}