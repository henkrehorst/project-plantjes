﻿using System;
using System.ComponentModel.DataAnnotations;

namespace project_c.Models.Users
{
    public class User
    {
        public int UserId { get; set; }
        [DataType(DataType.EmailAddress), StringLength(255), Required]
        public string Email { get; set; }
        
        [DataType(DataType.Password), Required, StringLength(64)]
        public string Password { get; set; }
        public enum Role { Customer, Admin }
        public int Status { get; set; }
        public DateTime Created { get; set; }

        public User(int userid, string email, string password, int status, DateTime created)
        {
            UserId = userid;
            Email = email;
            Password = password;
            Status = status;
            Created = created;
        }

}
}
