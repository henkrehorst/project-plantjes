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
            using (MailMessage message = new MailMessage("projectplantjes@gmail.com",  "projectplantjes@gmail.com"))
            {
                message.Subject = "User: " + model.Username + " Subj: " + model.Subject;
                message.Body = "\n" + model.Username + " zegt het volgende:\n\n" + model.Body;
                message.IsBodyHtml = false;

                using(SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential cred = new NetworkCredential("projectplantjes@gmail.com", "#1Geheim");
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = cred;
                    smtp.Port = 587;
                    smtp.Send(message);
                    ViewBag.Message = "Email Sent Successfully";

                    using (MailMessage aReply = new MailMessage("projectplantjes@gmail.com", "henkrehorst@outlook.com"))
                    {
                        aReply.Subject = "Wij hebben je mail ontvangen!";
                        aReply.Body = "\nHey " + model.Username + "\n\n We hebben je mail ontvangen en proberen deze zo snel mogelijk te beantwoorden!" +
                            "\n\n Jij zei het volgende:\n\n" + model.Subject + "\n\n" + model.Body + "\n________________________________________________\n\n" + 
                            "Wij verwachten je bericht uiterlijk binnnen 3 werkdagen te beantwoorden." +
                            "\n\n Groetjes!\n\n\n Het hele team van Planjes";
                        aReply.IsBodyHtml = false;

                        using (SmtpClient smtpReply = new SmtpClient())
                        {
                            smtpReply.Host = "smtp.gmail.com";
                            smtpReply.EnableSsl = true;
                            NetworkCredential credReply = new NetworkCredential("projectplantjes@gmail.com", "#1Geheim");
                            smtpReply.UseDefaultCredentials = true;
                            smtpReply.Credentials = credReply;
                            smtpReply.Port = 587;
                            smtpReply.Send(aReply);

                        }
                    }

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
