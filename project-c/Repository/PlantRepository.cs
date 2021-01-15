using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using project_c.Helpers;
using project_c.Models.Plants;

namespace project_c.Repository
{
    public class PlantRepository
    {
        public async Task<PaginatedResponse<Plant>> GetPlantsWithDistance(
            DataContext dataContext,
            double latitude,
            double longitude,
            int[] aanbod,
            int[] soort,
            int[] licht,
            int[] water,
            string name,
            int distance = 0,
            int page = 1,
            string sort = "new"
        )
        {
            //start join query
            var query = dataContext.Plants.Join(dataContext.User, plant => plant.UserId, user => user.Id, (plant, user) => new {plant, user});

            //build query
            if (aanbod.Length > 0) query = query.Where(t => aanbod.Contains(t.plant.Aanbod));
            if (soort.Length > 0) query = query.Where(t => soort.Contains(t.plant.Soort));
            if (licht.Length > 0) query = query.Where(t => licht.Contains(t.plant.Licht));
            if (water.Length > 0) query = query.Where(t => water.Contains(t.plant.Water));
            if (name != null)
                query = query.Where(t =>
                    EF.Functions.Like(t.plant.Name.ToLower(), $"%{name.ToLower()}%"));
            
            //show only plants in distance
            if (distance > 0 && longitude != 0.0 && latitude != 0.0) 
                query = query.Where(t => (t.user.Location.Distance(new Point(latitude, longitude)) / 1000 < distance));

            //show only approved plants
            query = query.Where(t => t.plant.HasBeenApproved);
            
            //sort plants by sort filter
            if (sort == "a-z")
            {
                query = query.OrderBy(t => t.plant.Name);
            }else if (sort == "z-a")
            {
                query = query.OrderByDescending(t => t.plant.Name);
            }else if (sort == "loc")
            {
                query = query.OrderBy(t => t.user.Location.Distance(new Point(latitude, longitude)));
            }
            else
            {
                query = query.OrderByDescending(t => t.plant.Creation);
            }

            //show only plant data and not user data
            var finalQuery  = query.Select(@t => new Plant(@t.plant, @t.user.Location.Distance(new Point(latitude, longitude)) / 1000));

            return await PaginatedResponse<Plant>.CreateAsync(finalQuery, page, 18);
        }
        
        public async Task<PaginatedResponse<Plant>> GetPlants(
            DataContext dataContext,
            int[] aanbod,
            int[] soort,
            int[] licht,
            int[] water,
            string name,
            int page = 1,
            string sort = "new"
        )
        {
            var query = dataContext.Plants.Select(p => p);

            //build query
            if (aanbod.Length > 0) query = query.Where(p => aanbod.Contains(p.Aanbod));
            if (soort.Length > 0) query = query.Where(p => soort.Contains(p.Soort));
            if (licht.Length > 0) query = query.Where(p => licht.Contains(p.Licht));
            if (water.Length > 0) query = query.Where(p => water.Contains(p.Water));
            if (name != null)
                query = query.Where(p =>
                    EF.Functions.Like(p.Name.ToLower(), $"%{name.ToLower()}%"));

            //show only approved plants
            query = query.Where(p => p.HasBeenApproved);

            //sort plants by sort filter
            if (sort == "a-z")
            {
                query = query.OrderBy(p => p.Name);
            }else if (sort == "z-a")
            {
                query = query.OrderByDescending(p => p.Name);
            }
            else
            {
                query = query.OrderByDescending(p => p.Creation);
            }

            return await PaginatedResponse<Plant>.CreateAsync(query, page, 18);
        }
    }
}