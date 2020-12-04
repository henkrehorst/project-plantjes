using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using project_c.Models.Users;

namespace project_c.Models.Plants
{
    public class Plant
    {
        [Key]
        public int PlantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }
        
        public string ImgUrl { get; set; }
        public bool HasBeenApproved { get; set; }
        public string UserId { get; set; }
        public User User { get; set; } 
    }
}