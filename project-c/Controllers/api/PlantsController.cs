using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using project_c.Helpers;
using project_c.Models.Plants;
using project_c.Repository;
using project_c.ViewModels;

namespace project_c.Controllers.api
{
    [Route("/api/[controller]")]
    [ApiController]
    public class PlantsController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly PlantRepository _plantRepository;

        public PlantsController(DataContext context, PlantRepository plantRepository)
        {
            this._dataContext = context;
            this._plantRepository = plantRepository;
        }

        //Get plants
        [HttpGet]
        public async Task<PaginatedResponse<Plant>> GetPlants(
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

            return latitude != 0.0 && longitude != 0.0 ? await _plantRepository.GetPlantsWithDistance(_dataContext,latitude, longitude, aanbod, soort, licht, water, name, distance, page, sort) : 
                await _plantRepository.GetPlants(_dataContext, aanbod, soort, licht, water, name, page, sort);
        }

        [HttpGet]
        [Route("/api/plants/images/{id}")]
        public async Task<string[]> GetPlantImages(int id)
        {
            var plant = await _dataContext.Plants.FirstOrDefaultAsync(p => p.PlantId == id);

            return plant.Images;
        }
    }
}