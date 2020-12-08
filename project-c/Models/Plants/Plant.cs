using System.ComponentModel.DataAnnotations;
using project_c.Models.Users;

namespace project_c.Models.Plants
{
    public class Plant
    {
        [Key] public int PlantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }
        public string ImgUrl { get; set; }

        //Category fields
        public int Aanbod { get; set; }

        public int Soort { get; set; }

        public int Licht { get; set; }

        public int Water { get; set; }
        //End Category fields
        
        public string UserId { get; set; }
        public User User { get; set; }
    }
}