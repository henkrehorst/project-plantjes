using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using project_c.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using project_c.Helpers;
using project_c.Models.Plants;
using project_c.Services.GeoRegister.Service;
using NetTopologySuite.Geometries;
using project_c.Repository;


namespace project_c.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _dataContext;
        private readonly PlantRepository _plantRepository;

        public HomeController(ILogger<HomeController> logger, DataContext dataContext, PlantRepository plantRepository)
        {
            _logger = logger;
            _dataContext = dataContext;
            _plantRepository = plantRepository;

        }

        public async Task<ViewResult> Index(
            [FromQuery(Name = "Aanbod")] int[] aanbod,
            [FromQuery(Name = "Soort")] int[] soort,
            [FromQuery(Name = "Licht")] int[] licht,
            [FromQuery(Name = "Water")] int[] water,
            [FromQuery(Name = "Naam")] string name,
            [FromQuery(Name = "postcode")] string zipcode,
            [FromQuery(Name = "lat")] double latitude,
            [FromQuery(Name = "lon")] double longitude,
            [FromQuery(Name = "Afstand")] int distance,
            [FromQuery(Name = "Sort")] string sort,
            [FromQuery(Name = "Page")] int page = 1)
        {
            //get filters 
            ViewData["Filters"] = _dataContext.Filters.Include(f => f.Options).ToList();
            
            return latitude != 0.0 && longitude != 0.0 ? View(await _plantRepository.GetPlantsWithDistance(_dataContext,latitude, longitude, aanbod, soort, licht, water, name, distance, page, sort)) : 
                View(await _plantRepository.GetPlants(_dataContext, aanbod, soort, licht, water, name, page, sort));

        }


        public IActionResult Profiel()
        {
            return View();
        }

        public IActionResult Ons()
        {
            return View();
        }
        public IActionResult Missie()
        {
            return View();
        }
        public IActionResult ContactPage()
        {
            return View();
        }
        public IActionResult Voorwaarden()
        {
            return View();
        }

        public IActionResult Routes()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}