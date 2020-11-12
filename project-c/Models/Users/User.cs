using System;

namespace project_c.Models.Users
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public char Password { get; set; }
        public enum Role { Customer, Admin }
        public int Status { get; set; }
        public DateTime Created { get; set; }

        public User(int userid, string email, char password, int status, DateTime created)
        {
            UserId = userid;
            Email = email;
            Password = password;
            Status = status;
            Created = created;
        }

}
}
