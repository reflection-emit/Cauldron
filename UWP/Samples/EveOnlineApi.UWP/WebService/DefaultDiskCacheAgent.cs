using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.Potions;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace EveOnlineApi.WebService
{
    public class DefaultDiskCacheAgent : ICachingAgent
    {
        public virtual async Task<object> GetCache<TResult>(string key)
        {
            var filename = key + ".cache";

            if (await ApplicationData.Current.LocalFolder.ExistsAsync(filename))
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                return await file.ReadTextAsync();
            }

            return "";
        }

        public virtual Task<bool> IsValid(string key) =>
            Task.FromResult(!Network.CreateInstance().HasInternetConnection);

        public virtual async Task SetCache<TResult>(string key, string content, TResult result)
        {
            var filename = key + ".cache";
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            await file.WriteTextAsync(content);
        }
    }
}