using Cauldron.Core;
using Cauldron.Core.Extensions;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace EveOnlineApi.WebService
{
    public sealed class FiveMinutesDiskCacheAgent : DefaultDiskCacheAgent
    {
        public override async Task<bool> IsValid(string key)
        {
            var filename = key + ".cache";

            if (await ApplicationData.Current.LocalFolder.ExistsAsync(filename))
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                return !Network.HasInternetConnection ||
                    await EveUtils.GetServerStatusAsync() != EveServerStatus.Online ||
                    (await file.GetDateModifiedAsync()).ToLocalTime().AddMinutes(5.0) > DateTime.Now;
            }

            return true;
        }
    }
}