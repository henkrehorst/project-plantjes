﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using project_c.Models.Plants;
using project_c.Services;

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
        public ActionResult Details(int id)
        {
            var plant = from p in _context.Plants where p.PlantId == id select p;
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
                plant.Name = name;
                plant.Length = Convert.ToInt32(form["length"]);
                plant.Description = description;
                
                if (image != null)
                {
                    plant.ImgUrl = await uploadService.UploadImage(image);
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
                var plant = _context.Plants.Find(id);
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