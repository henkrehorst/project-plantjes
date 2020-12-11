using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using project_c.Models.Plants;
using project_c.Models.Users;
using project_c.Services;
using project_c.ViewModels;


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
        public async Task<ActionResult> Details(int id)
        {
            var plant = (from p in this._context.Plants where p.PlantId == id select p).ToList();
            var ratings = from r in _context.Ratings where r.PlantId == id select r;

            var plantViewModel = new PlantViewModel();
            plantViewModel.Plant = plant;
            plantViewModel.Rating = ratings;
            plantViewModel.UserId = _userManager.GetUserId(User);

            return View(plantViewModel);
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
                if (_userManager.GetUserId(User) == plant.UserId || User.IsInRole("Admin"))
                {
                    _context.Plants.Remove(plant);
                    _context.SaveChanges();
                }
                else
                {
                    return Content("Your are not authorized to delete this plant");
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

        [HttpPost]
        public async Task<ActionResult> AddRating(int id, IFormCollection form)
        {
            var userId = _userManager.GetUserId(User);
            var data = from r in _context.Ratings where r.UserId == userId select r;
            var ratingValue = Convert.ToInt32(form["rating"]);
            var comment = form["comment"].ToString();
            var routingId = id;
            var noRating = true;

            PlantRating rating = new PlantRating();

            if (ModelState.IsValid)
            {
                rating.Rating = ratingValue;
                rating.Comment = comment;
                rating.PlantId = id;
                rating.UserId = userId;
            }

            foreach (var plantRating in data)
            {
                if (plantRating.PlantId == id)
                {
                    noRating = false;
                }
            }

            if (noRating)
            {
                _context.Add(rating);
                _context.SaveChanges();
            }
            else
            {
                return Content("You already voted");
            }

            return RedirectToAction("Details", new {id = routingId});
        }

        [HttpPost]
        public async Task<ActionResult> EditRating(int id, int routingId, IFormCollection form)
        {
            var ratingValue = Convert.ToInt32(form["rating"]);
            try
            {
                var rating = _context.Ratings.Find(id);

                if (_userManager.GetUserId(User) == rating.UserId)
                {
                    rating.Rating = ratingValue;
                    _context.Update(rating);
                    _context.SaveChanges();
                }
                else
                {
                    return Content("Your are not authorized to edit rating");
                }

                return RedirectToAction("Details", new {id = routingId});
            }
            catch 
            {
                return RedirectToAction("Details", new {id = routingId});
            }
        }
        
        [HttpPost]
        public async Task<ActionResult> DeleteRating(int id, int routingId, IFormCollection form)
        {
            try
            {
                var rating = _context.Ratings.Find(id);

                if (_userManager.GetUserId(User) == rating.UserId)
                {
                    
                    _context.Ratings.Remove(rating);
                    _context.SaveChanges();
                }
                else
                {
                    return Content("Your are not authorized to delete this rating");
                }

                return RedirectToAction("Details", new {id = routingId});
            }
            catch 
            {
                return RedirectToAction("Details", new {id = routingId});
            }
        }
    }
}