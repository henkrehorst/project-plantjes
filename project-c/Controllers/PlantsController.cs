using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using project_c.Models.Plants;

namespace project_c.Controllers
{
    public class PlantsController : Controller
    {
        private readonly DataContext _context;

        public PlantsController(DataContext context)
        {
            _context = context;
        }

        // GET: PlantsController
        public ActionResult Index()
        {
            var plants = from p in _context.Plants select p;
            return View(plants);
        }

        // GET: PlantsController/Details/5
        public ActionResult Details(int id)
        {
            var plant = from p in _context.Plants where p.PlantId == id select p;
            return View(plant);
        }

        // GET: PlantsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlantsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection form)
        {
            try
            {
                Plant plant = new Plant();
                plant.Name = form["name"];
                plant.Length = Convert.ToInt32(form["length"]);
                plant.Description = form["description"];
                _context.Add(plant);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        
        // // GET: PlantsController/Edit/5
        public ActionResult Edit(int id)
        {
            var plant = from p in _context.Plants where p.PlantId == id select p;
            return View(plant);
        }
        
        // POST: PlantsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection form)
        {
            try
            {
                var plant = _context.Plants.Find(id);
                
                plant.Name = form["name"];
                plant.Length = Convert.ToInt32(form["length"]);
                plant.Description = form["description"];
                _context.Update(plant);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        
        // // GET: PlantsController/Delete/5
        // public ActionResult Delete(int id)
        // {
        //     return View();
        // }
        //
        // // POST: PlantsController/Delete/5
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public ActionResult Delete(int id, IFormCollection collection)
        // {
        //     try
        //     {
        //         return RedirectToAction(nameof(Index));
        //     }
        //     catch
        //     {
        //         return View();
        //     }
        // }
    }
}
