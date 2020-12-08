using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_c.Models.Plants
{
    public class PlantOptions
    {
        public int OptionId { get; set; }
        public Option Option { get; set; }
        
        public int FilterId { get; set; }
        public Filter Filter { get; set; }
        
        public int PlantId { get; set; }
        public Plant Plant { get; set; }
    }
}