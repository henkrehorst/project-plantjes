using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace project_c.Services.GeoRegister.Client
{
    public class HttpClient: IClientInterface
    {
        private readonly System.Net.Http.HttpClient _httpClient;
        public HttpClient()
        {
            this._httpClient = new System.Net.Http.HttpClient();
        }
        
        public async Task<string> DoRequest(Endpoint.Endpoint endpoint)
        {
            var response= await _httpClient.GetAsync(endpoint.GetUrl());
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
    }
}