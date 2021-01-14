using project_c.Models.Plants;
using System.Collections.Generic;

namespace project_c.ViewModels
{
    public class UserReportViewModel
    {
        public IEnumerable<Report> Report { get; set; }
    }
}