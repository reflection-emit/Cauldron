using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EveOnlineApi.WebService
{
    internal sealed class DefaultRestClient : IServiceClient
    {
        private string authToken;
        private string host;

        public DefaultRestClient(string authToken, string host)
        {
            this.authToken = authToken;
            this.host = host;
        }

        public DefaultRestClient()
        {
        }

        public async Task<string> GetStringAsync(string requestUri)
        {
            try
            {
                HttpClient client = new HttpClient();

                if (!string.IsNullOrEmpty(this.authToken))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.authToken);

                if (!string.IsNullOrEmpty(this.host))
                    client.DefaultRequestHeaders.Host = this.host;

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