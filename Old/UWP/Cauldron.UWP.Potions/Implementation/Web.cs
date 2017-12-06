using Cauldron.Activator;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Cauldron.Potions.Implementation
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// A wrapper for <see cref="Cauldron.Core.Web"/>.
    /// See <see cref="Cauldron.Core.Web"/> for further details.
    /// </summary>
    [Component(typeof(IWeb), FactoryCreationPolicy.Instanced)]
    public sealed class Web : IWeb
    {
        [ComponentConstructor]
        private Web()
        {
        }

#if WINDOWS_UWP

        public Task DownloadFile(Uri uri, StorageFile resultFile) =>
            Core.Web.DownloadFile(uri, resultFile);

#else

        public Task DownloadFile(Uri uri, FileInfo resultFile) =>
            Core.Web.DownloadFile(uri, resultFile);

#endif
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}