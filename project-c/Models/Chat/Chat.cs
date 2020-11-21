using project_c.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace project_c.Models.Chat
{
    public class Chat
    {
        [Key]
        public static int ChatId { get; set; }

        public DateTime Created { get; set; }

        public List<Message> Messages {get; set;}

        public ChatData ChatData { get; set; }

          public void Send(User user, Message msg)
          {
            
          }
 }
}
