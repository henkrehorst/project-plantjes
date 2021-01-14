using project_c.Models.Plants;
using project_c.Models.Users;

namespace project_c.ViewModels
{
    public class CreateReportViewModel
    {
        public Plant Plant { get; set; }
        public User User { get; set; }

        public CreateReportViewModel(Plant plant, User user)
        {
            Plant = plant;
            User = user;
        }
    }
}