using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EveOnlineApi.WebService
{
    public sealed class AzureMobileServicesClient : IServiceClient
    {
        private string applicationKey;

        public AzureMobileServicesClient(string applicationKey, string host)
        {
            this.applicationKey = applicationKey;
        }

        public async Task<string> GetStringAsync(string requestUri)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-ZUMO-APPLICATION", this.applicationKey);
                var response = await client.GetAsync(requestUri);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.Accepted:
                        return await response.Content.ReadAsStringAsync();

                    case HttpStatusCode.NotImplemented:
                        throw new NotImplementedException(response.ReasonPhrase);

                    case HttpStatusCode.RequestTimeout:
                        throw new TimeoutException(response.ReasonPhrase);

                    case HttpStatusCode.Unauthorized:
                        throw new UnauthorizedAccessException(response.ReasonPhrase);

                    default:
                        throw new Exception(response.ReasonPhrase);
                }
            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }
    }
}