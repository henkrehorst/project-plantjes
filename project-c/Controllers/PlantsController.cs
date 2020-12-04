using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using project_c.Models.Plants;
using project_c.Services;

namespace project_c.Controllers
{
    public class PlantsController : Controller
    {
        private readonly DataContext _context;
        private readonly UploadService _uploadService;

        public PlantsController(DataContext context, UploadService upload)
        {
            _context = context;
            _uploadService = upload;
        }

        // GET: PlantsController
        public ActionResult Index(string naam)
        {
            var plants = from p in _context.Plants orderby p.PlantId descending select p;
            ViewData["Filters"] = _context.Filters.Include(f => f.Options).ToList();
            
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
            var plant = from p in _context.Plants where p.PlantId == id select p;
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
                    
                    _context.Add(plant);
                    _context.SaveChanges();
                    

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

                _context.Update(plant);
                _context.SaveChanges();
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
                var plant = _context.Plants.FirstOrDefault(p => p.PlantId == id);
                _context.Plants.Remove(plant);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Details));
            }
        }
    }
}