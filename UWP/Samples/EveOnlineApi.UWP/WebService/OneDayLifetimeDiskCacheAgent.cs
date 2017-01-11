using Cauldron.Core;
using Cauldron.Core.Extensions;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace EveOnlineApi.WebService
{
    public sealed class OneDayLifetimeDiskCacheAgent : DefaultDiskCacheAgent
    {
        public override async Task<bool> IsValid(string key)
        {
            var filename = key + ".cache";

            if (await ApplicationData.Current.LocalFolder.ExistsAsync(filename))
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                return !((await file.GetDateModifiedAsync()).AddDays(1) < DateTime.Now && Network.HasInternetConnection);
            }

            return !Network.HasInternetConnection;
        }
    }
}