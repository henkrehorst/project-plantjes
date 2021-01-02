using project_c.Models.Users;

namespace project_c.Models.Plants
{
    public class PlantRating
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public int PlantId { get; set; }
        public Plant Plant { get; set; }
        public string  UserId { get; set; }
        public User User { get; set; }
    }
}