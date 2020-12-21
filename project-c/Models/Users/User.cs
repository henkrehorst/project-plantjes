using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using project_c.Models.Chat;

namespace project_c.Models.Users
{
    public class User: IdentityUser
    {
        public UserData UserData { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
