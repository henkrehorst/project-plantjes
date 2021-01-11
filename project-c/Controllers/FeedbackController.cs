using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using project_c.Models;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using project_c.Helpers;
using project_c.Models.Plants;
using project_c.Services.GeoRegister.Service;
using NetTopologySuite.Geometries;
using project_c.Repository;

namespace project_c.Controllers
{
    public class FeedbackController : Controller
    {
        [BindProperty]
        public FeedbackModel Input { get; set; }

        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(IFormCollection form)
        {
            using (MailMessage message = new MailMessage("projectplantjes@gmail.com", "projectplantjes@gmail.com"))
            {
                message.Subject = "Feedback: " + Input.To;
                message.Body = "\n" + Input.To + " geeft als feedback:\n\n" + Input.Body;
                message.IsBodyHtml = false;

                if (ModelState.IsValid)
                {
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential cred = new NetworkCredential("projectplantjes@gmail.com", "#1Geheim");
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = cred;
                        smtp.Port = 587;
                        smtp.Send(message);
                        ViewBag.Message = "Email Sent Successfully";

                        using (MailMessage aReply = new MailMessage("projectplantjes@gmail.com", Input.To))
                        {
                            aReply.Subject = "Wij hebben je feedback ontvangen!";
                            aReply.Body = "\nHey " + Input.Naam +
                                          "\n\n Bedankt voor je feedback!" +
                                          "\n\n Jij zei het volgende:\n\n" + Input.Body +
                                          "\n________________________________________________\n\n" +
                                          "\n\n Groetjes!\n\n\n Het hele team van Planjes";
                            aReply.IsBodyHtml = false;

                            using (SmtpClient smtpReply = new SmtpClient())
                            {
                                smtpReply.Host = "smtp.gmail.com";
                                smtpReply.EnableSsl = true;
                                NetworkCredential credReply =
                                    new NetworkCredential("projectplantjes@gmail.com", "#1Geheim");
                                smtpReply.UseDefaultCredentials = true;
                                smtpReply.Credentials = credReply;
                                smtpReply.Port = 587;
                                smtpReply.Send(aReply);
                            }
                        }
                    }
                    return RedirectToAction("Index");
                }
                return View();
            }
            
        }
    }
}
