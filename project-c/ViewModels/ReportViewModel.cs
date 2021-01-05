using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project_c.Models.Plants;
using project_c.Models.Users;

namespace project_c.ViewModels
{
    public class ReportViewModel
    {
        public Plant Plant { get; set; }
        public User User { get; set; }

        public ReportViewModel(Plant plant, User user)
        {
            Plant = plant;
            User = user;
        }
    }
}
