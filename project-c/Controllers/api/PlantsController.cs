using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_c.Models.Plants;

namespace project_c.Controllers.api
{
    [Route("/api/[controller]")]
    [ApiController]
    public class PlantsController : Controller
    {
        private readonly DataContext _dataContext;

        public PlantsController(DataContext context)
        {
            this._dataContext = context;
        }

        //Get plants
        [HttpGet]
        public async Task<ActionResult<List<Plant>>> GetPlants(
            [FromQuery(Name = "Aanbod")] int[] aanbod,
            [FromQuery(Name = "Soort")] int[] soort,
            [FromQuery(Name = "Licht")] int[] licht,
            [FromQuery(Name = "Water")] int[] water
        )
        {
            var query = _dataContext.Plants.Select(p => p);

            //build query
            if (aanbod.Length > 0) query = query.Where(p => aanbod.Contains(p.Aanbod));
            if (soort.Length > 0) query = query.Where(p => soort.Contains(p.Soort));
            if (licht.Length > 0) query = query.Where(p => licht.Contains(p.Licht));
            if (water.Length > 0) query = query.Where(p => water.Contains(p.Water));

            return await query.Where(p => p.HasBeenApproved).ToListAsync();
        }
    }
}