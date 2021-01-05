using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using project_c.Services.GeoRegister.Client;
using project_c.Services.GeoRegister.Endpoint;
using project_c.Services.GeoRegister.Model;

namespace project_c.Services.GeoRegister.Handler
{
    public class ZipCodeHandler
    {
        private readonly ZipCodeEndpoint _zipCodeEndpoint;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ZipCodeHandler(HttpClient httpClient, IConfiguration configuration)
        {
            this._httpClient = httpClient;
            this._configuration = configuration;
            this._zipCodeEndpoint = new ZipCodeEndpoint(this._httpClient, this._configuration.GetConnectionString("GeoRegisterUrl"));
        }

        public async Task<Document> GetZipCodeInformation(string zipCode)
        {
            this._zipCodeEndpoint.SetSearchQuery(zipCode);
            
            var result = await RequestZipCodeInformation(zipCode);

            if (result != null)
            {
                if (result.NumFound > 0)
                {
                    return result.Docs[0];
                }
            }
            
            return null;
        }

        public async Task<Response> GetZipCodeResponse(string zipcode)
        {
            return await RequestZipCodeInformation(zipcode);
        }

        private async Task<Response> RequestZipCodeInformation(string zipCode)
        {
            this._zipCodeEndpoint.SetSearchQuery(zipCode);
            
            var result = await _zipCodeEndpoint.DoRequest();

            if (result != null)
            {
                return JsonConvert.DeserializeObject<Response>(JObject.Parse(result)["response"]?.ToString());
            }
            
            return null;
        }
    }
}