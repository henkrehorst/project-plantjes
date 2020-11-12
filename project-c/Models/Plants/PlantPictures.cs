using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_c.Models.Plants
{
    public class PlantPictures
    {
        public int PictureId { get; set; }
        public string Source { get; set; }
        public int Position { get; set; }
        public Plant RelatedPlant { get; set; }

        public PlantPictures (int pid, string source, int pos)
        {
            PictureId = pid;
            Source = source;
            Position = pos;
        }

        public void setPlant()
        {

        }
    }
}
