using System.Collections.Generic;
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
        public string FirstName { get; set; }
        
        [StringLength(45), Required] 
        public string LastName { get; set; }
        [Required]
        public string Zipcode { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required] 
        public string UserName { get; set; }
        public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }

    }
}