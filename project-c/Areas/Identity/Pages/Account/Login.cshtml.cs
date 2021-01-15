using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using project_c.Models.Users;

namespace project_c.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<User> signInManager,
            ILogger<LoginModel> logger,
            UserManager<User> userManager,
            DataContext context)

        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty] public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData] public string ErrorMessage { get; set; }

        public bool IsLoggedIn { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Geen Email ingevuld")]
            [EmailAddress]
            public string Email { get; set; }

            [Required(ErrorMessage = "Geen wachtwoord ingevuld")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Onthoud mij")] public bool RememberMe { get; set; }
        }
        
        public async Task OnGetAsync(string returnUrl = null)
        {
            var email = "";
            var password = "";
            
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            if (Request.Cookies != null)
            {
                foreach (var cookie in Request.Cookies)
                {
                    if (cookie.Key == "username")
                    {
                        email = cookie.Value;
                    }
                    if (cookie.Key == "password")
                    {
                        password = cookie.Value;
                    }
                }
            }
            returnUrl = returnUrl ?? Url.Content("~/");
            
            ViewData["email"] = email;
            ViewData["password"] = password;
            ViewData["isChecked"] = false;

            if (email != "" && password != "") { ViewData["isChecked"] = true; }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddDays(5);
            
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe,
                    lockoutOnFailure: true);
                var user = _context.User.Find(Input.Email);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    Program.IsLoggedIn = true;
                    var containsEmail = !Request.Cookies.ContainsKey("username");
                    var containsPassword = !Request.Cookies.ContainsKey("password");
                    if (Input.RememberMe)
                    {
                        if (containsEmail && containsPassword)
                        {
                            Response.Cookies.Append("username", Input.Email, options);
                            Response.Cookies.Append("password", Input.Password, options);
                        }
                    }
                    else
                    {
                        Response.Cookies.Delete("username");
                        Response.Cookies.Delete("password");
                    }
                    return LocalRedirect(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new {ReturnUrl = returnUrl, RememberMe = Input.RememberMe});
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }

                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty,
                        "Account is niet geactiveerd. Activeer u account via de email");
                    ViewData["isChecked"] = false;
                    return Page();
                }

                ModelState.AddModelError(string.Empty, "Het e-mailadres of wachtwoord is niet juist.");
                ViewData["isChecked"] = false;
                return Page();
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}