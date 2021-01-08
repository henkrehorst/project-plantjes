using project_c.Models.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_c.ViewModels
{
    public class UserReportViewModel
    {
        public IEnumerable<Report> Report { get; set; }
    }
}
