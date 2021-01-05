using System.Threading.Tasks;
using project_c.Services.GeoRegister.Handler;
using project_c.Services.GeoRegister.Model;

namespace project_c.Services.GeoRegister.Service
{
    public class ZipCodeService
    {
        private readonly ZipCodeHandler _zipCodeHandler;
        
        public ZipCodeService(ZipCodeHandler zipCodeHandler)
        {
            _zipCodeHandler = zipCodeHandler;
        }

        public async Task<Document> GetZipCodeInformation(string zipcode)
        {
            return await _zipCodeHandler.GetZipCodeInformation(zipcode);
        }

        public async Task<bool> CheckPostCodeIsValid(string zipcode)
        {
            var result = await _zipCodeHandler.GetZipCodeResponse(zipcode);

            if (result == null) return false;

            return result.NumFound >= 1;
        }


    }
}