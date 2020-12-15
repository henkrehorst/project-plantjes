using System.ComponentModel.DataAnnotations;

namespace project_c.Models.Users
{
    public class UserData
    {
        public int UserDataId { get; set; }
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

        public string UserId { get; set; }
        public User User { get; set; }

    }
}
