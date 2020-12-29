using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using project_c.Models.Users;


namespace project_c.Models.Chat
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        
        [Required]
        public string Text { get; set; }
        public DateTime When { get; set; }

        public string UserId { get; set; }
        
        public string ReceivedUserId { get; set; }
        public virtual User User { get; set; }
    }
}
