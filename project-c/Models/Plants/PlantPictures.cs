using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_c.Models.Plants
{
    public class PlantPictures
    {
        public int PlantPicturesId { get; set; }
        public string Source { get; set; }
        public int Position { get; set; }
        public int PlantId { get; set; }
        public Plant Plant { get; set; }
    }
}
