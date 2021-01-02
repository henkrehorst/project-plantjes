using System.Net.Http;
using System.Threading.Tasks;

namespace project_c.Services.GeoRegister.Client
{
    public interface IClientInterface
    {
        public Task<string> DoRequest(Endpoint.Endpoint endpoint);
    }
}