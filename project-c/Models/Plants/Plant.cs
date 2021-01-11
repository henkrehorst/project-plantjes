using System;
using System.ComponentModel.DataAnnotations;
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
        public string ImgUrl { get; set; }
        public string[] Images { get; set; }
        public bool HasBeenApproved { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        
        [NotMapped]
        public int Distance { get; set; }
        
        public Plant(){}

        public Plant(Plant plant, double distance)
        {
            PlantId = plant.PlantId;
            Name = plant.Name;
            Description = plant.Description;
            Length = plant.Length;
            Quantity = plant.Quantity;
            Aanbod = plant.Aanbod;
            Soort = plant.Soort;
            Licht = plant.Licht;
            Water = plant.Water;
            Creation = plant.Creation;
            ImgUrl = plant.ImgUrl;
            Images = plant.Images;
            HasBeenApproved = plant.HasBeenApproved;
            UserId = plant.UserId;
            User = plant.User;
            Distance = (int) distance != 0 ? (int) distance : 1;
        }
    }
}