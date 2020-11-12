using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace project_c_plantjes
{
    public class User
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public enum Role { customer, admin }
        public int Status { get; set; }
        public DateTime Created { get; set; }

        public User(int userid, string email, string password, int status, DateTime created)
        {
            UserID = userid;
            Email = email;
            Password = password;
            Status = status;
            Created = created;
        }

}
}
