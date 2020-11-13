using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_c.Models.Plants
{
    public class Plant
    {
        public int PlantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Length { get; set; }
        //public int Thumbnail { get; set; }
        //public User RelatedUser { get; set; }
        //The Photos field is not initialized by default.
        public List<PlantPictures> Photos { get; set; }

        public Plant(string name, string description, double length)
        {
            Name = name;
            Description = description;
            Length = length;
            //Thumbnail = thumb;
        }
    }
}
