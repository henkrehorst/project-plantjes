﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using project_c.Models.Users;
using project_c.Services;
using System.Net;
using System.Net.Mail;

namespace project_c.Models
{
    public class EmailModel
    {
        [Required(ErrorMessage = "Er is geen email ingevuld")]
        [EmailAddress(ErrorMessage = "Er is geen geldig email adres ingevuld"), DisplayName("Email")]
        public string To { get; set; }
        [Required(ErrorMessage = "Vul een onderwerp in"), DisplayName("Onderwerp")]
        [DataType(DataType.Text)]
        public string Subject { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }
        [Required(ErrorMessage ="Er is geen bericht ingevuld.")]
        [DataType(DataType.Text), DisplayName("Bericht")]
        public string Body { get; set; }
        [Required]
        [EmailAddress]
        public string FromEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string FromPassword { get; set; }

     
    }
}
