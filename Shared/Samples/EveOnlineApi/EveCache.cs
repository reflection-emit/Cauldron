using Cauldron.Core;
using Cauldron.Core.Collections;
using Cauldron.Core.Extensions;
using EveOnlineApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace EveOnlineApi
{
    internal static class EveCache
    {
        internal static CacheCollection Cache { get; private set; } = new CacheCollection();

        internal static async Task<T> GetCache<T>() => await GetCache<T>(typeof(T).Name);

        internal static async Task<T> GetCache<T>(string key)
        {
            if (Cache.Contains(key))
                return (T)Cache[key].Object;

            var filename = key + ".json";

            if (!await ApplicationData.Current.TemporaryFolder.ExistsAsync(filename))
                return default(T);

            var content = await ApplicationData.Current.TemporaryFolder.ReadTextAsync(filename);
            if (content == null)
                throw new Exception("Something is wrong with the cache");

            var obj = JsonConvert.DeserializeObject<T>(content);
            var cachedUntil = DateTime.Now.AddDays(7);

            var enumerable = obj as IEnumerable;

            if (enumerable == null)
            {
                if (obj is IKeyedModel)
                    cachedUntil = (obj as IKeyedModel).CachedUntil;
                else if (obj is IXmlModelCollection)
                    cachedUntil = (obj as IXmlModelCollection).CachedUntil;
            }

            Cache.Add(new DataCacheItem { CachedUntil = cachedUntil, Name = key, Object = obj });
            return (T)Cache[key].Object;
        }

        internal static async Task<IEnumerable<T>> GetCacheCollection<T>() => await GetCache<IEnumerable<T>>(typeof(T).Name);

        internal static async Task SetCache(object obj)
        {
            string name;
            var cachedUntil = DateTime.Now.AddDays(7);
            var type = obj.GetType();
            var enumerable = obj as IEnumerable;

            if (enumerable != null)
            {
                var childType = type.GetChildrenType();
                name = childType.Name;
            }
            else if (obj is IKeyedModel)
            {
                var keyedModel = obj as IKeyedModel;
                name = keyedModel.Key;
                cachedUntil = keyedModel.CachedUntil;
            }
            else if (obj is IXmlModelCollection)
            {
                var keyedModel = obj as IXmlModelCollection;
                name = keyedModel.Key;
                cachedUntil = keyedModel.CachedUntil;
            }
            else
                name = type.Name;

            // Update the cache
            if (Cache.Contains(name))
                Cache.Remove(name);

            Cache.Add(new DataCacheItem { CachedUntil = cachedUntil, Name = name, Object = obj });

            var filename = name + ".json";
            var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
            var content = JsonConvert.SerializeObject(obj);

            await file.WriteTextAsync(content);
        }
    }

    internal sealed class CacheCollection : ConcurrentKeyedCollection<string, DataCacheItem>
    {
        protected override string GetKeyForItem(DataCacheItem item)
        {
            return item.Name;
        }
    }

    internal sealed class DataCacheItem
    {
        public DateTime CachedUntil { get; set; }

        public string Name { get; set; }

        public object Object { get; set; }
    }
}