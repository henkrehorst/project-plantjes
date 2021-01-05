using project_c.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_c.Models.Plants
{
    public class Report
    {
        public int ReportId { get; set; }
        public string Body { get; set; }
        public Plant Plant { get; set; }
        public User User { get; set; }
    }
}
