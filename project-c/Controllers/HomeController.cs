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
using Microsoft.EntityFrameworkCore;
using project_c.Helpers;
using project_c.Models.Plants;


namespace project_c.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _dataContext;

        public HomeController(ILogger<HomeController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public async Task<ViewResult> Index(
            [FromQuery(Name = "Aanbod")] int[] aanbod,
            [FromQuery(Name = "Soort")] int[] soort,
            [FromQuery(Name = "Licht")] int[] licht,
            [FromQuery(Name = "Water")] int[] water,
            [FromQuery(Name = "Naam")] string name,
            [FromQuery(Name = "Page")] int page = 1)
        {
            //get filters 
            ViewData["Filters"] = _dataContext.Filters.Include(f => f.Options).ToList();
            
            var query = _dataContext.Plants.Select(p => p);
            
            //build query
            if (aanbod.Length > 0) query = query.Where(p => aanbod.Contains(p.Aanbod));
            if (soort.Length > 0) query = query.Where(p => soort.Contains(p.Soort));
            if (licht.Length > 0) query = query.Where(p => licht.Contains(p.Licht));
            if (water.Length > 0) query = query.Where(p => water.Contains(p.Water));
            if (name != null)
                query = query.Where(p =>
                    EF.Functions.Like(p.Name.ToLower(), $"%{name.ToLower()}%"));
            
            //show only approved plants
            query = query.Where(p => p.HasBeenApproved);

            ViewData["stekCount"] = query.Count();

            return View(await PaginatedResponse<Plant>.CreateAsync(query, page, 15));
        }

        [HttpPost]
        public IActionResult Faq(EmailModel model)
        {
            using (MailMessage message = new MailMessage("projectplantjes@gmail.com", "projectplantjes@gmail.com"))
            {
                message.Subject = "User: " + model.To + " Subj: " + model.Subject;
                message.Body = "\n" + model.To + " zegt het volgende:\n\n" + model.Body;
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

                    using (MailMessage aReply = new MailMessage("projectplantjes@gmail.com", model.To))
                    {
                        aReply.Subject = "Wij hebben je mail ontvangen!";
                        aReply.Body = "\nHey " + model.To +
                                      "\n\n We hebben je mail ontvangen en proberen deze zo snel mogelijk te beantwoorden!" +
                                      "\n\n Jij zei het volgende:\n\n" + model.Subject + "\n\n" + model.Body +
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

        public ViewResult Faq()
        {
            return View();
        }
        
        public async Task<IActionResult> Profiel()
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