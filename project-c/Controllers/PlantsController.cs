using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using project_c.Helpers;
using project_c.Models;
using project_c.Repository;
using project_c.ViewModels;

namespace project_c.Controllers
{
    public class PlantsController : Controller
    {
        private readonly DataContext _context;
        private readonly UploadService _uploadService;
        private readonly UserManager<User> _userManager;
        private readonly PlantRepository _plantRepository;

        [BindProperty] public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Voer de naam van de plant in!")]
            [StringLength(40,
                ErrorMessage = "De naam van de plant mag maximaal 40 tekens en moet minimaal 3 tekens bevatten!",
                MinimumLength = 3)]
            public string Name { get; set; }

            [Required(ErrorMessage = "Voer de lengte van de plant in!")]
            public int Length { get; set; }

            [Required(ErrorMessage = "Voer het aantal planten in!")]
            public int Amount { get; set; }

            [Required(ErrorMessage = "Selecteer het aanbod waar de plant in hoort!")]
            public int Aanbod { get; set; }

            [Required(ErrorMessage = "Selecteer het soort plant!")]
            public int Soort { get; set; }

            [Required(ErrorMessage = "Selecteer de hoeveelheid licht die de plant nodig heeft!")]
            public int Licht { get; set; }

            [Required(ErrorMessage = "Selecteer de hoeveelheid water die de plant nodig heeft!")]
            public int Water { get; set; }

            public bool checkBees { get; set; }
            public bool checkOtherAnimals { get; set; }
            public bool checkOtherPlants { get; set; }

            [Required(ErrorMessage = "Maak korte beschrijving over de plant")]
            [StringLength(200,
                ErrorMessage = "De beschrijving moet minimaal 10 tekens bevatten en mag maximaal 250 tekens bevatten",
                MinimumLength = 10)]
            public string Description { get; set; }

            public string ImageOrder { get; set; }

            [DataType(DataType.Upload)]
            [Required(ErrorMessage = "U moet minimaal 1 foto uploaden")]
            [MaxFileSizeArray(2 * 1024 * 1024)]
            [AllowedExtensionsArray(new string[] {".jpg"})]
            public IFormFile[] PlantPictures { get; set; }
        }

        //te doen - zorg ervoor dat de aantal en uploadsdatum te zien zijn voor andere gebruikers - zorg ook voor checks of ze er zijn wanneer je dit doet.
        public PlantsController(DataContext context, UserManager<User> userManager, UploadService upload,
            PlantRepository plantRepository)
        {
            _context = context;
            _userManager = userManager;
            _uploadService = upload;
            _plantRepository = plantRepository;
        }

        // GET: PlantsController
        public async Task<ViewResult> Index(
            [FromQuery(Name = "Aanbod")] int[] aanbod,
            [FromQuery(Name = "Soort")] int[] soort,
            [FromQuery(Name = "Licht")] int[] licht,
            [FromQuery(Name = "Water")] int[] water,
            [FromQuery(Name = "Naam")] string name,
            [FromQuery(Name = "postcode")] string zipcode,
            [FromQuery(Name = "lat")] double latitude,
            [FromQuery(Name = "lon")] double longitude,
            [FromQuery(Name = "Afstand")] int distance,
            [FromQuery(Name = "Sort")] string sort,
            [FromQuery(Name = "Page")] int page = 1)
        {
            //get filters 
            ViewData["Filters"] = _context.Filters.Include(f => f.Options).ToList();

            ViewBag.plantIsEdited = TempData["plantIsEdited"] == null ? false : TempData["plantIsEdited"];
            ViewBag.ratingIsCreated = TempData["ratingIsCreated"] == null ? false : TempData["ratingIsCreated"];
            ViewBag.ratingIsDeleted = TempData["ratingIsDeleted"] == null ? false : TempData["ratingIsDeleted"];
            ViewBag.ratingIsEdited = TempData["ratingIsEdited"] == null ? false : TempData["ratingIsEdited"];
            ViewBag.reportIsSubmitted = TempData["reportIsSubmitted"] == null ? false : TempData["reportIsSubmitted"];

            return latitude != 0.0 && longitude != 0.0
                ? View(await _plantRepository.GetPlantsWithDistance(_context, latitude, longitude, aanbod, soort, licht,
                    water, name, distance, page, sort))
                : View(await _plantRepository.GetPlants(_context, aanbod, soort, licht, water, name, page, sort));
        }

        // GET: PlantsController/Details/5
        public ActionResult Details(int id)
        {
            var singlePlant = _context.Plants.Include(u => u.User).FirstOrDefault(p => p.PlantId == id);

            var aanbod = (from o in _context.Options
                where singlePlant.Aanbod == o.OptionId
                select o.DisplayName).FirstOrDefault();
            var soort = (from o in _context.Options
                where singlePlant.Soort == o.OptionId
                select o.DisplayName).FirstOrDefault();
            var licht = (from o in _context.Options
                where singlePlant.Licht == o.OptionId
                select o.DisplayName).FirstOrDefault();
            var water = (from o in _context.Options
                where singlePlant.Water == o.OptionId
                select o.DisplayName).FirstOrDefault();

            var categories = new List<string>() {aanbod, soort, licht, water};

            var ratings = _context.Ratings.Where(r => r.PlantId == id);

            var plantViewModel = new PlantViewModel();
            plantViewModel.Plant = new List<Plant>() {singlePlant};
            plantViewModel.Rating = ratings;
            plantViewModel.UserId = _userManager.GetUserId(User);
            plantViewModel.Categories = categories;

            ViewBag.plantIsEdited = TempData["plantIsEdited"] == null ? false : TempData["plantIsEdited"];
            ViewBag.ratingIsCreated = TempData["ratingIsCreated"] == null ? false : TempData["ratingIsCreated"];
            ViewBag.ratingIsDeleted = TempData["ratingIsDeleted"] == null ? false : TempData["ratingIsDeleted"];
            ViewBag.ratingIsEdited = TempData["ratingIsEdited"] == null ? false : TempData["ratingIsEdited"];

            return View(plantViewModel);
        }

        // GET: PlantsController/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewData["Filters"] = this._context.Filters.Include(filter => filter.Options).ToList();
            ViewBag.plantIsNotCreated = TempData["plantIsNotCreated"] == null ? false : TempData["plantIsNotCreated"];
            return View();
        }

        // POST: PlantsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create(IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Plant plant = new Plant();

                    if (ModelState.IsValid)
                    {
                        plant.Name = char.ToUpper(Input.Name[0]) + Input.Name.Substring(1);
                        ;
                        plant.Length = Convert.ToInt32(form["length"]);
                        plant.Description = char.ToUpper(Input.Description[0]) + Input.Description.Substring(1);
                        ;
                        plant.Quantity = Input.Amount;

                        var imageOrder = JsonConvert.DeserializeObject<ImageOrderModel[]>(Input.ImageOrder);

                        //convert uploaded images in image order
                        var imagesArray = new string[imageOrder.Length];

                        for (int i = 0; i < imageOrder.Length; i++)
                        {
                            var plantPicture =
                                Input.PlantPictures.FirstOrDefault(f => f.FileName == imageOrder[i].Location);
                            if (plantPicture != null)
                            {
                                imagesArray[i] = await _uploadService.UploadImage(plantPicture);
                            }
                        }

                        //remove null values from array
                        imagesArray = imagesArray.Where(x => x != null).ToArray();

                        plant.Images = imagesArray;
                        plant.ImgUrl = imagesArray[0];

                        //added categories of plant
                        plant.Aanbod = Input.Aanbod;
                        plant.Soort = Input.Soort;
                        plant.Licht = Input.Licht;
                        plant.Water = Input.Water;

                        plant.checkBees = Input.checkBees;
                        plant.checkOtherAnimals = Input.checkOtherAnimals;
                        plant.checkOtherPlants = Input.checkOtherPlants;

                        plant.Creation = DateTime.Now;
                        plant.UserId = _userManager.GetUserId(User);
                        User plantuser = _context.User.First(u => u.Id == plant.UserId);
                        if (plantuser.Karma >= 3)
                        {
                            plant.HasBeenApproved = true;
                        }

                        _context.Add(plant);
                        _context.SaveChanges();
                    }

                    TempData["plantIsCreated"] = true;
                    return RedirectToAction("MijnPlanten");
                }
                catch
                {
                    TempData["plantIsNotCreated"] = true;
                    return RedirectToAction("Create");
                }
            }

            ViewData["Filters"] = this._context.Filters.Include(filter => filter.Options).ToList();
            return View();
        }

        public string FetchUser(Plant plant)
        {
            User usr = _context.User.Single(x => x.Id == plant.UserId);
            return usr.UserName;
        }

        // //GET: PlantsController/Report/5
        public ActionResult Report(int id)
        {
            Plant plant = _context.Plants.First(p => p.PlantId == id);
            User user = _context.User.Single(u => u.Id == plant.UserId);


            return View(new CreateReportViewModel(plant, user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> MakeReport(int PlantId, IFormCollection form)
        {
            //tedoen - zorg ervoor dat de plantid goed naar deze method verstuurd word.
            var body = form["Body"].ToString();
            var sender = await _userManager.GetUserAsync(User);
            var plant = _context.Plants.First(p => p.PlantId == PlantId);
            try
            {
                Report report = new Report();

                report.Body = body;
                report.Plant = plant;
                report.User = await _userManager.GetUserAsync(User);

                _context.Add(report);
                _context.SaveChanges();    
                
                TempData["reportIsSubmitted"] = true;
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // // GET: PlantsController/Edit/5
        [Authorize]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Plant plant = _context.Plants.First(p => p.PlantId == id);
            ViewData["Filters"] = this._context.Filters.Include(filter => filter.Options).ToList();
            Input = new InputModel();
            Input.Name = plant.Name;
            Input.Amount = plant.Quantity;
            Input.Length = plant.Length;
            Input.Aanbod = plant.Aanbod;
            Input.Soort = plant.Soort;
            Input.Water = plant.Water;
            Input.Licht = plant.Licht;
            Input.checkBees = plant.checkBees;
            Input.checkOtherAnimals = plant.checkOtherAnimals;
            Input.checkOtherPlants = plant.checkOtherPlants;
            Input.Description = plant.Description;

            return View(this);
        }

        // POST: PlantsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit(int id, IFormCollection form)
        {
            var imageOrder = JsonConvert.DeserializeObject<ImageOrderModel[]>(Input.ImageOrder);

            // ignore upload error if no images uploaded and old images exists
            if (imageOrder != null && imageOrder.Length > 0 && Input.PlantPictures == null)
            {
                if (ModelState.ContainsKey("Input.PlantPictures"))
                    ModelState.Remove("Input.PlantPictures");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var plant = _context.Plants.First(p => p.PlantId == id);

                    if (_userManager.GetUserId(User) == plant.UserId || User.IsInRole("Admin"))
                    {
                        plant.Name = char.ToUpper(Input.Name[0]) + Input.Name.Substring(1);
                        plant.Length = Convert.ToInt32(form["length"]);
                        plant.Description = char.ToUpper(Input.Description[0]) + Input.Description.Substring(1);
                        plant.Quantity = Input.Amount;

                        //convert uploaded images in image order
                        var imagesArray = new string[imageOrder.Length];
                        for (int i = 0; i < imageOrder.Length; i++)
                        {
                            if (imageOrder[i].AlreadyExists)
                            {
                                imagesArray[i] = imageOrder[i].Location;
                            }
                            else
                            {
                                var plantPicture =
                                    Input.PlantPictures.FirstOrDefault(f => f.FileName == imageOrder[i].Location);
                                if (plantPicture != null)
                                {
                                    imagesArray[i] = await _uploadService.UploadImage(plantPicture);
                                }
                            }
                        }

                        //remove null values from array
                        imagesArray = imagesArray.Where(x => x != null).ToArray();

                        plant.Images = imagesArray;
                        plant.ImgUrl = imagesArray[0];

                        //added categories of plant
                        plant.Aanbod = Input.Aanbod;
                        plant.Soort = Input.Soort;
                        plant.Licht = Input.Licht;
                        plant.Water = Input.Water;

                        plant.checkBees = Input.checkBees;
                        plant.checkOtherAnimals = Input.checkOtherAnimals;
                        plant.checkOtherPlants = Input.checkOtherPlants;

                        plant.Creation = DateTime.Now;
                        User plantuser = _context.User.First(u => u.Id == plant.UserId);
                        if (plantuser.Karma >= 3)
                        {
                            plant.HasBeenApproved = true;
                        }

                        _context.Update(plant);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return Content("Your are not authorized to edit this plant");
                    }

                    TempData["plantIsEdited"] = true;
                    return RedirectToAction("Details", new {id});
                }
                catch
                {
                    ViewData["Filters"] = this._context.Filters.Include(filter => filter.Options).ToList();
                    return View();
                }
            }

            ViewData["Filters"] = this._context.Filters.Include(filter => filter.Options).ToList();
            return View();
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
                var reports = _context.Reports.Where(r => r.Plant.PlantId == id);
                if (User.IsInRole("Admin"))
                {
                    //send email here
                    using (MailMessage message = new MailMessage("plantjesbuurt@gmail.com", usr.Email))
                    {
                        message.Subject = $"Uw plant {plant.Name} is niet goedgekeurd";
                        message.Body = $"Beste {usr.FirstName} , \n\n\n" +
                                       $"In verband met onze siteregels is uw plant {plant.Name} helaas niet goedgekeurd. \n" +
                                       "Neem a.u.b de regels opnieuw door voordat u het opnieuw probeert. \n\n" +
                                       "Groetjes, Plantjesbuurt";
                        message.IsBodyHtml = false;
                        using (SmtpClient smtp = new SmtpClient())
                        {
                            smtp.Host = "smtp.gmail.com";
                            smtp.EnableSsl = true;
                            NetworkCredential cred = new NetworkCredential("plantjesbuurt@gmail.com", "#1Geheim");
                            smtp.UseDefaultCredentials = true;
                            smtp.Credentials = cred;
                            smtp.Port = 587;
                            smtp.Send(message);
                        }
                    }

                    foreach (Report rep in reports)
                    {
                        _context.Reports.Remove(rep);
                    }

                    _context.Plants.Remove(plant);
                    usr.Karma--;
                    _context.SaveChanges();
                    TempData["plantIsDeleted"] = true;
                    return RedirectToAction("MijnPlanten");
                }
                else if (_userManager.GetUserId(User) == plant.UserId)
                {
                    foreach (Report rep in reports)
                    {
                        _context.Reports.Remove(rep);
                    }

                    _context.Plants.Remove(plant);
                    _context.SaveChanges();
                }
                else
                {
                    return Content("You are not authorized to perform this action.");
                }

                TempData["plantIsDeleted"] = true;
                return RedirectToAction("MijnPlanten");
            }
            catch
            {
                return RedirectToAction("MijnPlanten");
            }
        }

        [Authorize]
        public async Task<ActionResult> MijnPlanten()
        {
            var plants = _context.Plants.Where(p => p.UserId == _userManager.GetUserId(User))
                .OrderByDescending(p => p.Creation);
            ViewData["User"] = await _userManager.GetUserAsync(User);
            ViewBag.plantIsCreated = TempData["plantIsCreated"] == null ? false : TempData["plantIsCreated"];
            ViewBag.plantIsDeleted = TempData["plantIsDeleted"] == null ? false : TempData["plantIsDeleted"];

            return View(plants);
        }

        [HttpPost]
        public ActionResult AddRating(int id, IFormCollection form)
        {
            var userId = _userManager.GetUserId(User);
            var data = from r in _context.Ratings where r.UserId == userId select r;
            var ratingValue = Convert.ToInt32(form["rating"]);
            var comment = form["comment"].ToString();
            var routingId = id;
            var noRating = true;

            PlantRating rating = new PlantRating();


            rating.Rating = ratingValue;
            rating.Comment = comment;
            rating.PlantId = id;
            rating.UserId = userId;


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

            TempData["ratingIsCreated"] = true;
            return RedirectToAction("Details", new {id = routingId});
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

                return RedirectToAction("Index","Home");
            }
            catch
            {
                return RedirectToAction(nameof(Details));
            }
        }

        [HttpPost]
        public ActionResult EditRating(int id, int routingId, IFormCollection form)
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

                TempData["ratingIsEdited"] = true;
                return RedirectToAction("Details", new {id = routingId});
            }
            catch
            {
                return RedirectToAction("Details", new {id = routingId});
            }
        }

        [HttpPost]
        public ActionResult DeleteRating(int id, int routingId, IFormCollection form)
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

                TempData["ratingIsDeleted"] = true;
                return RedirectToAction("Details", new {id = routingId});
            }
            catch
            {
                return RedirectToAction("Details", new {id = routingId});
            }
        }
    }
}