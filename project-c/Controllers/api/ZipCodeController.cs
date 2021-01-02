using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using project_c.Services.GeoRegister.Service;

namespace project_c.Controllers.api
{
    [ApiController]
    public class ZipCodeController : Controller
    {
        private class ZipCodeResponse
        {
            public string ZipCode { get; set; }

            public double Latitude { get; set; }

            public double Longitude { get; set; }
        }

        private readonly ZipCodeService _zipCodeService;

        public ZipCodeController(ZipCodeService zipCodeService)
        {
            _zipCodeService = zipCodeService;
        }


        [HttpGet]
        [Route("/api/[controller]/{zipCode}")]
        public async Task<ActionResult> GetZipCodeInformation(string zipCode)
        {
            //get zipcode information
            var response = await _zipCodeService.GetZipCodeInformation(zipCode);

            if (response == null) return NotFound();

            return Ok(new ZipCodeResponse()
                {Latitude = response.Latitude, Longitude = response.Longitude, ZipCode = zipCode});
        }
    }
}