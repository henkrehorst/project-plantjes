using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace project_c_plantjes.Models.Users
{
    public class UserData
    {
        public int UserDataID { get; set; }
        [StringLength(45), Required]
        public string FirstName { get; set; }
        [StringLength(45), Required]
        public string LastName { get; set; }
        [DataType(DataType.PostalCode), Required]
        public string ZipCode { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        [DataType(DataType.Url)]
        public string Avatar { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

    }
}
