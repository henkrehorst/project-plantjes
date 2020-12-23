using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query;
using project_c.Models.Plants;
using project_c.Models.Users;

namespace project_c.ViewModels
{
    public class PlantViewModel
    {
        public List<Plant> PlantUserData { get; set; }

        public List<string> Categories { get; set; }
    }
}