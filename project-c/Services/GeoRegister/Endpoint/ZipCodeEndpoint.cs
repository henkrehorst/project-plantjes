using project_c.Services.GeoRegister.Client;

namespace project_c.Services.GeoRegister.Endpoint
{
    public class ZipCodeEndpoint : Endpoint
    {
        public ZipCodeEndpoint(HttpClient httpClient, string url) : base(httpClient, url)
        {
        }

        public void SetSearchQuery(string zipCode)
        {
            this.SetQuery($"q=postcode:{zipCode}&rows=1"); 
        }
    }
}