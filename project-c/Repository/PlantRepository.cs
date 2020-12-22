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
            int page = 1
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
            
            //order
            query = query.OrderByDescending(t => t.user.Location.Distance(new Point(latitude, longitude)));
            
            //show only plant data and user data
            var finalQuery  = query.Select(@t => new Plant(@t.plant, @t.user.Location.Distance(new Point(latitude, longitude)) / 1000));

            return await PaginatedResponse<Plant>.CreateAsync(finalQuery, page, 15);
        }
        
        public async Task<PaginatedResponse<Plant>> GetPlants(
            DataContext dataContext,
            int[] aanbod,
            int[] soort,
            int[] licht,
            int[] water,
            string name,
            int page = 1
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

            return await PaginatedResponse<Plant>.CreateAsync(query, page, 15);
        }
    }
}