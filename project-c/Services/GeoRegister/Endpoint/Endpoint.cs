using System.Net.Http;
using System.Threading.Tasks;
using project_c.Services.GeoRegister.Client;

namespace project_c.Services.GeoRegister.Endpoint
{
    public abstract class Endpoint
    {
        private readonly IClientInterface _client;
        private string _url;
        private string _query;

        public Endpoint(IClientInterface client, string url)
        {
            this._client = client;
            this._url = url;
        }

        public string GetUrl()
        {
            return $"{this._url}?{this._query}";
        }

        public async Task<string> DoRequest()
        {
            return await _client.DoRequest(this);
        }

        public void SetQuery(string query)
        {
            this._query = query;
        }
    }
}