using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace project_c.Models.Chat
{
    public class Message
    {
        public Chat Chat { get; set; }
        [Key]
        public int MessageId { get; set; }

        [StringLength(255)]
        public string MessageContent { get; set; }
    }
}
