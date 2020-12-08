﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using project_c.Models.Plants;
using project_c.Models.Users;
using project_c.Services;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace project_c.Controllers
{
    public class PlantsController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        public PlantsController(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: PlantsController
        public ActionResult Index(string naam)
        {

            var a = _context.User.Count();

            var plants = from p in _context.Plants orderby p.PlantId descending select p;
            if (!String.IsNullOrEmpty(naam))
            {
                naam = char.ToUpper(naam[0]) + naam.Substring(1);
                var plant = from p in _context.Plants where p.Name.Contains(naam) select p;
                return View(plant);
            }
            return View(plants);
        }

        // GET: PlantsController/Details/5
        public ActionResult Details(int id)
        {
            var plant = _context.Plants.Where(p => p.PlantId == id).Include(p => p.User).ThenInclude(u => u.UserData);
            return View(plant);
        }

        // GET: PlantsController/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlantsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create(IFormCollection form)
        {
            
            var name = form["name"].ToString();
            var description = form["description"].ToString();
            description = char.ToUpper(description[0]) + description.Substring(1);
            name = char.ToUpper(name[0]) + name.Substring(1);
            IFormFile image = form.Files.GetFile("ImageUpload");
            UploadService uploadService = new UploadService();
            try
            {
                Plant plant = new Plant();
                if (ModelState.IsValid)
                {
                    plant.Name = name;
                    plant.ImgUrl = await uploadService.UploadImage(image);
                    plant.Length = Convert.ToInt32(form["length"]);
                    plant.Description = description;
                    plant.UserId = _userManager.GetUserId(User);
                    UserData plantuserdata = _context.UserData.Single(z => z.UserId == plant.UserId);
                    if (plantuserdata.Karma >= 3)
                    {
                        plant.HasBeenApproved = true;
                    }
                    _context.Add(plant);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Content("Error probeer het opnieuw");
            }
        }

        public string FetchUser(Plant plant)
        {
            User usr = _context.User.Single(x => x.Id == plant.UserId);
            return usr.UserName;
        }

        // // GET: PlantsController/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            var plant = from p in _context.Plants where p.PlantId == id select p;
            return View(plant);
        }

        // POST: PlantsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit(int id, IFormCollection form)
        {
            var name = form["name"].ToString();
            var description = form["description"].ToString();
            description = char.ToUpper(description[0]) + description.Substring(1);
            name = char.ToUpper(name[0]) + name.Substring(1);
            IFormFile image = form.Files.GetFile("ImageUpload");
            UploadService uploadService = new UploadService();
            
            try
            {
                var plant = _context.Plants.Find(id);
                if (_userManager.GetUserId(User) == plant.UserId || User.IsInRole("Admin"))
                {
                    plant.Name = name;
                    plant.Length = Convert.ToInt32(form["length"]);
                    plant.Description = description; 
                    if (image != null)
                    {
                        plant.ImgUrl = await uploadService.UploadImage(image);
                    }
                    _context.Update(plant);
                    _context.SaveChanges();
                }
                else
                {
                    return Content("Your are not authorized to edit this plant");
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: PlantsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Delete(int id)
        {
            try
            {
                var plant = _context.Plants.Find(id);
                User usr = _context.User.Single(y => y.Id == plant.UserId);
                UserData usrdat = _context.UserData.Single(y => y.UserId == usr.Id);
                if (User.IsInRole("Admin"))
                { 
                    //send email here
                    using(MailMessage message = new MailMessage("projectplantjes@gmail.com", usr.Email))
                    {
                        message.Subject = $"Uw plant {plant.Name} is niet goedgekeurd";
                        message.Body = $"Beste {usrdat.FirstName} , \n\n\n" +
                            $"In verband met onze siteregels is uw plant {plant.Name} helaas niet goedgekeurd. \n" +
                            "Neem a.u.b de regels opnieuw door voordat u het opnieuw probeert. \n\n" +
                            "Groetjes, Het Plantjes Team";
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
                        }
                    }
                    _context.Plants.Remove(plant);
                    usrdat.Karma--;
                    _context.SaveChanges();
                }
                else if(_userManager.GetUserId(User) == plant.UserId){
                    _context.Plants.Remove(plant);
                    _context.SaveChanges();
                }
                else
                {
                    return Content("You are not authorized to perform this action.");
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Details));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Approve(int id) 
        {
            try
            {
                var plant = _context.Plants.Find(id);
                User plantuser = _context.User.Single(y => y.Id == plant.UserId);
                UserData plantuserdata = _context.UserData.Single(z => z.UserId == plantuser.Id);
                if (_userManager.GetUserId(User) == plant.UserId || User.IsInRole("Admin"))
                {
                    plant.HasBeenApproved = true;
                    plantuserdata.Karma++;
                    _context.SaveChanges();
                }
                else
                {
                    return Content("You are not authorized to perform this action");
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Details));
            }
        }

        public ActionResult MijnPlanten()
        {
            var plants = from p in _context.Plants where p.UserId == _userManager.GetUserId(User) select p;
            
            return View(plants);
        }
    }
}