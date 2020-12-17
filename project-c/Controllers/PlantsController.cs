using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using project_c.Models.Plants;
using project_c.Models.Users;
using project_c.Services;
using System.Net.Mail;
using System.Net;
using project_c.Helpers;

namespace project_c.Controllers
{
    public class PlantsController : Controller
    {
        private readonly DataContext _context;
        private readonly UploadService _uploadService;
        private readonly UserManager<User> _userManager;

        public PlantsController(DataContext context, UserManager<User> userManager, UploadService upload)
        {
            _context = context;
            _userManager = userManager;
            _uploadService = upload;
        }

        // GET: PlantsController
        public async Task<ViewResult> Index(
            [FromQuery(Name = "Aanbod")] int[] aanbod,
            [FromQuery(Name = "Soort")] int[] soort,
            [FromQuery(Name = "Licht")] int[] licht,
            [FromQuery(Name = "Water")] int[] water,
            [FromQuery(Name = "Naam")] string name,
            [FromQuery(Name = "Page")] int page = 1)
        {
            //get filters 
            ViewData["Filters"] = _context.Filters.Include(f => f.Options).ToList();
            
            var query = _context.Plants.Select(p => p);
            
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
        // GET: PlantsController/Details/5
        public ActionResult Details(int id)
        {
            var plant = _context.Plants.Where(p => p.PlantId == id).Include(p => p.User);
            return View(plant);
        }

        // GET: PlantsController/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewData["Filters"] = this._context.Filters.Include(filter => filter.Options).ToList();
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

            try
            {
                Plant plant = new Plant();
                if (ModelState.IsValid)
                {
                    plant.Name = name;
                    plant.ImgUrl = await _uploadService.UploadImage(image);
                    plant.Length = Convert.ToInt32(form["length"]);
                    plant.Description = description;
                    
                    //added categories of plant
                    plant.Aanbod = Convert.ToInt32(form["filter[Aanbod]"]);
                    plant.Soort = Convert.ToInt32(form["filter[Soort]"]);
                    plant.Licht = Convert.ToInt32(form["filter[Licht]"]);
                    plant.Water = Convert.ToInt32(form["filter[Water]"]);

                    plant.UserId = _userManager.GetUserId(User);
                    User plantuser = _context.User.First(u => u.Id == plant.UserId);
                    if (plantuser.Karma >= 3)
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
            Plant plant = _context.Plants.First(p => p.PlantId == id);
            ViewData["Filters"] = this._context.Filters.Include(filter => filter.Options).ToList();
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

            try
            {

                var plant = _context.Plants.First(p => p.PlantId == id);

                plant.Name = name;
                plant.Length = Convert.ToInt32(form["length"]);
                plant.Description = description;
                
                //added categories of plant
                plant.Aanbod = Convert.ToInt32(form["filter[Aanbod]"]);
                plant.Soort = Convert.ToInt32(form["filter[Soort]"]);
                plant.Licht = Convert.ToInt32(form["filter[Licht]"]);
                plant.Water = Convert.ToInt32(form["filter[Water]"]);
                
                if (image != null)
                {
                    plant.ImgUrl = await _uploadService.UploadImage(image);
				}
				
                if (_userManager.GetUserId(User) == plant.UserId || User.IsInRole("Admin"))
                {
                    plant.Name = name;
                    plant.Length = Convert.ToInt32(form["length"]);
                    plant.Description = description; 
                    if (image != null)
                    {
                        plant.ImgUrl = await _uploadService.UploadImage(image);
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
                if (User.IsInRole("Admin"))
                { 
                    //send email here
                    using(MailMessage message = new MailMessage("projectplantjes@gmail.com", usr.Email))
                    {
                        message.Subject = $"Uw plant {plant.Name} is niet goedgekeurd";
                        message.Body = $"Beste {usr.FirstName} , \n\n\n" +
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
                    usr.Karma--;
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
                if (_userManager.GetUserId(User) == plant.UserId || User.IsInRole("Admin"))
                {
                    plant.HasBeenApproved = true;
                    plantuser.Karma++;
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