using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project_c.Models;
using project_c.Models.Users;

namespace project_c.Controllers
{
    public class FaqController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<User> _userManager;

        public FaqController(DataContext dataContext,
            UserManager<User> userManager)
        {
            this._dataContext = dataContext;
            this._userManager = userManager;
        }

        public EmailModel Input { get; set; }

        public ViewResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _dataContext.User.FirstOrDefault(u => u.Id == _userManager.GetUserId(User));

                if (user != null) return View(new EmailModel() {To = user.Email});
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                using (MailMessage message = new MailMessage("projectplantjes@gmail.com", "projectplantjes@gmail.com"))
                {
                    message.Subject = "User: " + Input.To + " Subj: " + Input.Subject;
                    message.Body = "\n" + Input.To + " zegt het volgende:\n\n" + Input.Body;
                    message.IsBodyHtml = false;

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
                            aReply.Subject = "Wij hebben je mail ontvangen!";
                            aReply.Body = "\nHey " + Input.To +
                                          "\n\n We hebben je mail ontvangen en proberen deze zo snel mogelijk te beantwoorden!" +
                                          "\n\n Jij zei het volgende:\n\n" + Input.Subject + "\n\n" + Input.Body +
                                          "\n________________________________________________\n\n" +
                                          "Wij verwachten je bericht uiterlijk binnnen 3 werkdagen te beantwoorden." +
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
                }

                return RedirectToAction("Index");
            }

            return View();
        }
    }
}