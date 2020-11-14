using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
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
        
        // GET
        public IActionResult Index()
        {
            var plants = from p in _context.Plants select p;
            return View(plants);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string name, int length, string description)
        {
            Plant plant = new Plant();
            plant.Name = name;
            plant.Length = length;
            plant.Description = description;

            _context.Add(plant);
            _context.SaveChanges();
            return Content("Plant toegevoegd");
        }
    }
}