using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_c.Models.Chat
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            Messages = new HashSet<Message>();
        }
        // 1 - * AppUser || Message
        public virtual ICollection<Message> Messages { get; set; }
    }
}
