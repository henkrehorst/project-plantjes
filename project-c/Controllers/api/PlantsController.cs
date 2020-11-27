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

        // Get plants
        [HttpGet]
        public async Task<ActionResult<List<Plant>>> GetPlants()
        {
            var result = await (_dataContext.Plants
                .Join(_dataContext.PlantOptions, p => p.PlantId, o => o.PlantId, (p, o) => new {p, o})
                .Where(@t => @t.o.OptionId == 23)
                .Select(@t => new Plant()
                {
                    PlantId = @t.p.PlantId, Name = @t.p.Name, ImgUrl = @t.p.ImgUrl, Description = @t.p.Description
                })).ToListAsync();
            
            //no plant fond return not found
            if (result.Count == 0) return NotFound();
            
            return result;
        }
    }
}