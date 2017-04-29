using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Extensions;
using EveOnlineApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EveOnlineApi.WebService
{
    internal interface ICachingAgent
    {
        Task<object> GetCache<TResult>(string key);

        Task<bool> IsValid(string key);

        Task SetCache<TResult>(string key, string content, TResult result);
    }

    internal interface IDeserializer
    {
        TResult DeserializeObject<TResult>(string content);
    }

    internal interface IServiceClient
    {
        Task<string> GetStringAsync(string requestUri);
    }

    internal static class WebServiceConsumer
    {
        public static async Task<TResult> Consume<TClient, TDeserializer, TCachingAgent, TResult>(string requestUri, string clientParameter = null, string host = null)
            where TClient : IServiceClient
            where TDeserializer : class, IDeserializer
            where TCachingAgent : class, ICachingAgent
        {
            string response = string.Empty;

            try
            {
                var caching = typeof(TCachingAgent).CreateInstance() as TCachingAgent;
                var client = (TClient)typeof(TClient).CreateInstance(clientParameter, host);
                var deserializer = typeof(TDeserializer).CreateInstance() as TDeserializer;
                var key = requestUri.GetHash(HashAlgorithms.Md5);

                if (await caching.IsValid(key))
                {
                    try
                    {
                        var data = await caching.GetCache<TResult>(key);

                        if (data is TResult)
                            return (TResult)data;
                        else if (data == null)
                            return default(TResult);
                        else if (data is string)
                            return deserializer.DeserializeObject<TResult>(data as string);

                        return default(TResult);
                    }
                    catch
                    {
                        return await GetData<TResult>(client, deserializer, caching, key, requestUri);
                    }
                }
                else
                {
                    return await GetData<TResult>(client, deserializer, caching, key, requestUri);
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WebServiceConsumerException("An error has occured while trying to consume service", e)
                {
                    Url = requestUri,
                    Response = response
                };
            }
        }

        public static async Task<TResult> Consume<TClient, TDeserializer, TResult>(string requestUri, string clientParameter = null, string host = null)
            where TClient : IServiceClient
            where TDeserializer : class, IDeserializer
        {
            string response = string.Empty;

            try
            {
                var client = (TClient)typeof(TClient).CreateInstance(clientParameter, host);
                var deserializer = Factory.Create<TDeserializer>();

                response = await client.GetStringAsync(requestUri);

                return deserializer.DeserializeObject<TResult>(response);
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WebServiceConsumerException("An error has occured while trying to consume service", e)
                {
                    Url = requestUri,
                    Response = response
                };
            }
        }

        private static async Task<TResult> GetData<TResult>(IServiceClient client, IDeserializer deserializer, ICachingAgent caching, string key, string requestUri)
        {
            try
            {
                var response = await client.GetStringAsync(requestUri);

                var result = deserializer.DeserializeObject<TResult>(response);
                await caching.SetCache<TResult>(key, response, result);

                return result;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (HttpRequestException)
            {
                var data = await caching.GetCache<TResult>(key);

                if (data is TResult)
                    return (TResult)data;

                return deserializer.DeserializeObject<TResult>(data as string);
            }
        }
    }
}