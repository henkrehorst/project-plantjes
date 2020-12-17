using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace project_c.Models.Users
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; }
        [StringLength(45), Required]
        public string LastName { get; set; }
        [DataType(DataType.PostalCode), Required]
        public string ZipCode { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        [DataType(DataType.Url)]
        public string Avatar { get; set; }
        public int Karma { get; set; }
    }
}
