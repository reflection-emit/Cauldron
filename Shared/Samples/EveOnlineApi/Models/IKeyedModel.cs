using System;

namespace EveOnlineApi.Models
{
    public interface IKeyedModel
    {
        DateTime CachedUntil { get; set; }
        string Key { get; set; }
    }
}