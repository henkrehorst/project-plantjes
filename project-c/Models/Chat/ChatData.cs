using project_c.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_c.Models.Chat
{
    public class ChatData
    {
        public ICollection<Message> Messages { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
