using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using project_c.Models.Plants;

namespace project_c.Controllers.admin
{
    public class FilterController : Controller
    {
        private readonly DataContext _dbContext;

        public FilterController(DataContext dataContext)
        {
            this._dbContext = dataContext;
        }

        [BindProperty] public InputModel FormInput { get; set; }

        public class InputModel
        {
            [Required, MinLength(5)] public string Name { get; set; }

            [Required] public string Description { get; set; }

            [Required] public List<OptionInput> Options { get; set; }
        }

        public class OptionInput
        {
            [Required, MinLength(4)] public string DisplayName { get; set; }

            public int Position { get; set; }

            public int OptionId { get; set; }
        }


        [Route("Admin/Filter")]
        // GET: Filter
        public ActionResult Index()
        {
            ViewData["Filters"] = this._dbContext.Filters.ToList();
            return View("~/Views/Admin/Filter/Index.cshtml");
        }

        [Route("Admin/Filter/Create")]
        // GET: Filter/Create
        public ActionResult Create()
        {
            ViewData["OptionCount"] = 1;
            return View("~/Views/Admin/Filter/Create.cshtml");
        }

        // POST: Filter/Create
        [Route("Admin/Filter/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            if (ModelState.IsValid)
            {
                Filter filter = new Filter()
                {
                    Name = FormInput.Name,
                    Description = FormInput.Description,
                    Position = _dbContext.Filters.Count(),
                    Options = new List<Option>()
                };

                foreach (var option in FormInput.Options)
                {
                    filter.Options.Add(new Option()
                    {
                        DisplayName = option.DisplayName,
                        Position = option.Position
                    });
                }

                _dbContext.Add(filter);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewData["OptionCount"] = FormInput.Options.Count;
            return View("~/Views/Admin/Filter/Create.cshtml");
        }

        [Route("Admin/Filter/Edit/{id}")]
        // GET: Filter/Edit/5
        public ActionResult Edit(int id)
        {
            return View("~/Views/Admin/Filter/Edit.cshtml");
        }

        [Route("Admin/Filter/Field/{pos}")]
        // GET: Filter/Edit/5
        public ActionResult AddOptionField(int pos)
        {
            ViewData["pos"] = pos;
            return View("~/Views/Admin/Filter/_OptionField.cshtml");
        }

        // POST: Filter/Edit/5
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public ActionResult Edit(int id, IFormCollection collection)
        // {
        //     try
        //     {
        //         // TODO: Add update logic here
        //
        //         return RedirectToAction(nameof(Index));
        //     }
        //     catch
        //     {
        //         return View();
        //     }
        // }
    }
}