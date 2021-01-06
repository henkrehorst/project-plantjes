using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using project_c.Models.Chat;
using System.Collections.Generic;
using project_c.Models.Plants;

namespace project_c.Models.Users
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; }
        [StringLength(45), Required]
        public string LastName { get; set; }
        [DataType(DataType.PostalCode), Required]
        public string ZipCode { get; set; }
        
        [Column(TypeName = "geography")]
        public Point Location { get; set; }
        
        [DataType(DataType.Url)]
        public string Avatar { get; set; }
        
        public string ProfileBanner { get; set; }
        
        public string Bio { get; set; }
        public int Karma { get; set; }
		public virtual ICollection<Message> Messages { get; set; }
        
		public virtual ICollection<Plant> Plants { get; set; }
    }
}
