using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using project_c.Models;
using project_c.Services;
using System.Net;


namespace project_c.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public ViewResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Faq(EmailModel model)
        {
            using (MailMessage message = new MailMessage(model.FromEmail,  model.To))
            {
                message.Subject = model.Subject;
                message.Body = model.Body;
                message.IsBodyHtml = false;

                using(SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential cred = new NetworkCredential(model.FromEmail, model.FromPassword);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = cred;
                    smtp.Port = 587;
                    smtp.Send(message);
                    ViewBag.Message = "Email Sent Successfully";

                }
            }    
                return View();
        }
        public ViewResult Faq()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
