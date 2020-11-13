using Microsoft.AspNetCore.Identity;

namespace project_c.Models.Users
{
    public class User: IdentityUser
    {
        public UserData UserData { get; set; }
    }
}
