using Cauldron.Potions;
using EveOnlineApi.Models;
using System;
using System.Threading.Tasks;

namespace EveOnlineApi.WebService
{
    public sealed class EveXmlSpecializedCacheAgent<T> : ICachingAgent where T : IKeyedModel, new()
    {
        public Task<object> GetCache<TResult>(string key)
        {
            return Task.FromResult<object>(EveCache.GetCache<T>(key));
        }

        public async Task<bool> IsValid(string key)
        {
            var name = typeof(T).Name;

            if (EveCache.Cache.Contains(name))
                return !Network.CreateInstance().HasInternetConnection || await EveUtils.GetServerStatusAsync() != EveServerStatus.Online || EveCache.Cache[name].CachedUntil.ToLocalTime() > DateTime.Now;

            return !Network.CreateInstance().HasInternetConnection || await EveUtils.GetServerStatusAsync() != EveServerStatus.Online;
        }

        public async Task SetCache<TResult>(string key, string content, TResult result)
        {
            var data = result as IKeyedModel;

            if (data == null)
                return;

            data.Key = key;
            await EveCache.SetCache(data);
        }
    }
}