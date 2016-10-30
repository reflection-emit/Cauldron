using Cauldron.Activator;
using Cauldron.Core.Extensions;
using Cauldron.Potions;
using EveOnlineApi.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace EveOnlineApi.WebService
{
    public class MotdCachingAgent : DefaultDiskCacheAgent
    {
        public override async Task<bool> IsValid(string key)
        {
            if (!Network.CreateInstance().HasInternetConnection)
                return true;

            var filename = key + ".cache";

            if (await ApplicationData.Current.LocalFolder.ExistsAsync(filename))
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                var motd = JsonConvert.DeserializeObject<Motd>(await file.ReadTextAsync());
                var server = Factory.Create<IEveApi>().Server;

                if (motd.ServerName == server.ServerName)
                    return true;
            }

            return false;
        }
    }
}