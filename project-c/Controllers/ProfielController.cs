using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_c.Models.Users;

namespace project_c.Controllers
{
    public class ProfielController: Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<User> _userManager;

        public ProfielController(DataContext dataContext, UserManager<User> userManager)
        {
            this._dataContext = dataContext;
            _userManager = userManager;
        }
        
        
        [Authorize]
        public ViewResult Index()
        {
            //get userid
            var userid = _userManager.GetUserId(User);
            //get user with plants
            var user = _dataContext.User.Include(u => u.Plants).FirstOrDefault(u => u.Id == userid);
            
            return View(user);
        }
        
        [Authorize]
        [Route("/profiel/{userId}")]
        public ActionResult Details(string userId)
        {
            ViewData["CurrentUserId"] = _userManager.GetUserId(User);
            
            //get user with plants
            var user = _dataContext.User
                .Include(u => u.Plants)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}