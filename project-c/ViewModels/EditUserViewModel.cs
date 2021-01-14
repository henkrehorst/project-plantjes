using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using project_c.Models.Users;

namespace project_c.ViewModels
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }

        public string Id { get; set; }
        
        [StringLength(45), Required]
        [DisplayName("Voornaam")]
        public string FirstName { get; set; }
        
        [StringLength(45), Required]
        [DisplayName("Achternaam")]
        public string LastName { get; set; }
        [Required]
        [DisplayName("Postcode")]
        public string Zipcode { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DisplayName("Gebruikersnaam")]
        public string UserName { get; set; }
        [Required]
        public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }

    }
}