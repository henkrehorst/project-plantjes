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
            if (!String.IsNullOrEmpty(naam))
            {
                naam = char.ToUpper(naam[0]) + naam.Substring(1);
                var plant = from p in _context.Plants where p.Name.Contains(naam) select p;
                return View(plant);
            }

            ViewData["Filters"] = _context.Filters.Include(f => f.Options).ToList();

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

            //TODO: add later form binding and fix filter array
            string[] Filters = form["filters[]"].ToString().Split(',');
            Dictionary<int, int> FilterDict = new Dictionary<int, int>();
            foreach (string filter in Filters)
            {
                string[] splitFilterAndOptionId = filter.Split('-');
                FilterDict.Add(Convert.ToInt32(splitFilterAndOptionId[0]), Convert.ToInt32(splitFilterAndOptionId[1]));
            }

            try
            {
                Plant plant = new Plant();
                if (ModelState.IsValid)
                {
                    plant.Name = name;
                    plant.ImgUrl = await _uploadService.UploadImage(image);
                    plant.Length = Convert.ToInt32(form["length"]);
                    plant.Description = description;
                    _context.Add(plant);
                    _context.SaveChanges();

                    //added plant options
                    foreach (var item in FilterDict)
                    {
                        _context.Add(new PlantOptions()
                            {FilterId = item.Key, OptionId = item.Value, PlantId = plant.PlantId});
                    }

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
            Plant plant = _context.Plants.Include(p => p.PlantOptions).First(p => p.PlantId == id);
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
            //TODO: add later form binding and fix filter array
            string[] Filters = form["filters[]"].ToString().Split(',');
            Dictionary<int, int> FilterDict = new Dictionary<int, int>();
            foreach (string filter in Filters)
            {
                string[] splitFilterAndOptionId = filter.Split('-');
                FilterDict.Add(Convert.ToInt32(splitFilterAndOptionId[0]), Convert.ToInt32(splitFilterAndOptionId[1]));
            }

            try
            {
                var plant = _context.Plants.Include(p => p.PlantOptions).First(p => p.PlantId == id);

                plant.Name = name;
                plant.Length = Convert.ToInt32(form["length"]);
                plant.Description = description;

                if (image != null)
                {
                    plant.ImgUrl = await _uploadService.UploadImage(image);
                }

                //update filter options
                //added plant options
                foreach (var item in FilterDict)
                {
                    //check plant has already this filter
                    if (plant.PlantOptions.Count(o => o.FilterId == item.Key) > 0)
                    {
                        //update option in plant filter
                        foreach (var plantOption in plant.PlantOptions.Where(o => o.FilterId == item.Key))
                        {
                            plantOption.OptionId = item.Value;
                        }
                    }
                    else
                    {
                        //add new filter
                        _context.Add(new PlantOptions()
                            {PlantId = plant.PlantId, FilterId = item.Key, OptionId = item.Value});
                    }
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
                var plant = _context.Plants.Include(p => p.PlantOptions).FirstOrDefault(p => p.PlantId == id);
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