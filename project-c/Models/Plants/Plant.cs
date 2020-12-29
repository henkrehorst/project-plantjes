using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using project_c.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_c.Models.Plants
{
    public class Plant
    {
        [Key]
        public int PlantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }
        public int Quantity { get; set; }
        
        //Category fields

        public int Aanbod { get; set; }
        public int Soort { get; set; }
        public int Licht { get; set; }
        public int Water { get; set; }
        //End Category fields
        
        public DateTime Creation { get; set; }
        public string[] Images { get; set; }
        public bool HasBeenApproved { get; set; }
        public string UserId { get; set; }
        public User User { get; set; } 
    }
}