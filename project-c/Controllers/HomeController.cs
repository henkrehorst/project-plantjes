using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using project_c.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using project_c.Helpers;
using project_c.Models.Plants;
using project_c.Services.GeoRegister.Service;
using NetTopologySuite.Geometries;
using project_c.Repository;


namespace project_c.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _dataContext;
        private readonly PlantRepository _plantRepository;

        public HomeController(ILogger<HomeController> logger, DataContext dataContext, PlantRepository plantRepository)
        {
            _logger = logger;
            _dataContext = dataContext;
            _plantRepository = plantRepository;

        }

        public async Task<ViewResult> Index(
            [FromQuery(Name = "Aanbod")] int[] aanbod,
            [FromQuery(Name = "Soort")] int[] soort,
            [FromQuery(Name = "Licht")] int[] licht,
            [FromQuery(Name = "Water")] int[] water,
            [FromQuery(Name = "Naam")] string name,
            [FromQuery(Name = "postcode")] string zipcode,
            [FromQuery(Name = "lat")] double latitude,
            [FromQuery(Name = "lon")] double longitude,
            [FromQuery(Name = "Afstand")] int distance,
            [FromQuery(Name = "Sort")] string sort,
            [FromQuery(Name = "Page")] int page = 1)
        {
            //get filters 
            ViewData["Filters"] = _dataContext.Filters.Include(f => f.Options).ToList();
            
            return latitude != 0.0 && longitude != 0.0 ? View(await _plantRepository.GetPlantsWithDistance(_dataContext,latitude, longitude, aanbod, soort, licht, water, name, distance, page, sort)) : 
                View(await _plantRepository.GetPlants(_dataContext, aanbod, soort, licht, water, name, page, sort));

        }

        [HttpPost]
        public IActionResult Faq(EmailModel model)
        {
            using (MailMessage message = new MailMessage("plantjesbuurt@gmail.com", "plantjesbuurt@gmail.com"))
            {
                message.Subject = "User: " + model.To + " Subj: " + model.Subject;
                message.Body = "\n" + model.To + " zegt het volgende:\n\n" + model.Body;
                message.IsBodyHtml = false;

                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential cred = new NetworkCredential("plantjesbuurt@gmail.com", "#1Geheim");
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = cred;
                    smtp.Port = 587;
                    smtp.Send(message);
                    ViewBag.Message = "Email Sent Successfully";

                    using (MailMessage aReply = new MailMessage("plantjesbuurt@gmail.com", model.To))
                    {
                        aReply.Subject = "Wij hebben je mail ontvangen!";
                        aReply.Body = "\nHey " + model.To +
                                      "\n\n We hebben je mail ontvangen en proberen deze zo snel mogelijk te beantwoorden!" +
                                      "\n\n Jij zei het volgende:\n\n" + model.Subject + "\n\n" + model.Body +
                                      "\n________________________________________________\n\n" +
                                      "Wij verwachten je bericht uiterlijk binnnen 3 werkdagen te beantwoorden." +
                                      "\n\n Groetjes!\n\n\n Het hele team van Plantjesbuurt";
                        aReply.IsBodyHtml = false;

                        using (SmtpClient smtpReply = new SmtpClient())
                        {
                            smtpReply.Host = "smtp.gmail.com";
                            smtpReply.EnableSsl = true;
                            NetworkCredential credReply =
                                new NetworkCredential("plantjesbuurt@gmail.com", "#1Geheim");
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

        [HttpPost]
        public IActionResult Feedback(FeedbackModel model)
        {
            using (MailMessage message = new MailMessage("plantjesbuurt@gmail.com", "plantjesbuurt@gmail.com"))
            {
                message.Subject = "Feedback: " + model.To;
                message.Body = "\n" + model.To + " geeft als feedback:\n\n"  + model.Body;
                message.IsBodyHtml = false;

                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential cred = new NetworkCredential("plantjesbuurt@gmail.com", "#1Geheim");
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = cred;
                    smtp.Port = 587;
                    smtp.Send(message);
                    ViewBag.Message = "Email Sent Successfully";

                    using (MailMessage aReply = new MailMessage("plantjesbuurt@gmail.com", model.To))
                    {
                        aReply.Subject = "Wij hebben je feedback ontvangen!";
                        aReply.Body = "\nHey " + model.Naam +
                                      "\n\n Bedankt voor je feedback!" +
                                      "\n\n Jij zei het volgende:\n\n" + model.Body +
                                      "\n________________________________________________\n\n" +
                                      "\n\n Groetjes!\n\n\n Het hele team van Planjesbuurt";
                        aReply.IsBodyHtml = false;

                        using (SmtpClient smtpReply = new SmtpClient())
                        {
                            smtpReply.Host = "smtp.gmail.com";
                            smtpReply.EnableSsl = true;
                            NetworkCredential credReply =
                                new NetworkCredential("plantjesbuurt@gmail.com", "#1Geheim");
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

        public ViewResult Feedback()
        {
            return View();
        }
        public ViewResult Faq()
        {
            return View();
        }
        
        public IActionResult Profiel()
        {
            return View();
        }

        public IActionResult Ons()
        {
            return View();
        }
        public IActionResult Voorwaarden()
        {
            return View();
        }

        public IActionResult Routes()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}