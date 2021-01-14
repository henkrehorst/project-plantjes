using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project_c.Models.Users;

namespace project_c.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;

            public AccountController(SignInManager<User> signInManager)
            {
                _signInManager = signInManager;
            }

            [HttpPost]
            public async Task<IActionResult> Logout()
            {
                await _signInManager.SignOutAsync();
                return LocalRedirect("/Identity/Account/Logout");
            }

            public ActionResult RegisterConfirmation()
            {
                // ReSharper disable once Mvc.ViewNotResolved
                return View();
            }
    }
}