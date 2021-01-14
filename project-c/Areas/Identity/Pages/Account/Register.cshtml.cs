using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using project_c.Models.Users;
using project_c.Services;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using NetTopologySuite.Geometries;
using NpgsqlTypes;
using project_c.Services.GeoRegister.Service;
using EmailModel = project_c.Models.EmailModel;

namespace project_c.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _EmailModel;
        private readonly ZipCodeService _zipCodeService;
        private readonly DataContext _context;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender EmailModel,
            RoleManager<IdentityRole> roleManager,
            ZipCodeService zipCodeService,
            DataContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _EmailModel = EmailModel;
            _roleManager = roleManager;
            _zipCodeService = zipCodeService;
            _context = context;
        }

        [BindProperty] public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Geen Email ingevuld")]
            [EmailAddress(ErrorMessage = "Vul een geldig Emailadres in")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Geen voornaam ingevuld")]
            [DataType(DataType.Text)]
            [Display(Name = "Voornaam")]
            public string Voornaam { get; set; }

            [Required(ErrorMessage = "Geen achternaam ingevuld")]
            [DataType(DataType.Text)]
            [Display(Name = "Achternaam")]
            public string Achternaam { get; set; }

            [Required(ErrorMessage = "Geen postcode ingevuld")]
            [StringLength(6, ErrorMessage = "Ongeldige Postcode", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Postcode")]
            public string PostCode { get; set; }

            [Required(ErrorMessage = "Geen wachtwoord ingevuld")]
            [StringLength(100, ErrorMessage = "Het  {0} moet minstens {2} en maximaal {1} karakters lang zijn.",
                MinimumLength = 6)]
            [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
                ErrorMessage =
                    "Het wachtwoord voldoet niet aan de eisen: het wachtwoord moet minimaal 1 hoofdletter bevatten," +
                    " minimaal 1 cijfer, minimaal 1 speciaal teken en moet minimaal 8 tekens lang zijn.")]
            [DataType(DataType.Password)]
            [Display(Name = "Wachtwoord")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Geen wachtwoord ingevuld")]
            [DataType(DataType.Password)]
            [Display(Name = "Verifieer wachtwoord")]
            [Compare("Password", ErrorMessage = "De wachtwoorden zijn niet gelijk.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            returnUrl = returnUrl ?? Url.Content("~/Identity/Account/RegisterConfirmation");
            ReturnUrl = Url.Content("~/Identity/Account/RegisterConfirmation");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!await _zipCodeService.CheckPostCodeIsValid(Input.PostCode))
            {
                ModelState.AddModelError("Input.PostCode", "Deze postcode is ongeldig, probeer een andere!");
            }

            if (ModelState.IsValid)
            {
                var zipCodeInformation = await _zipCodeService.GetZipCodeInformation(Input.PostCode);

                var user = new User
                {
                    UserName = Input.Email, Email = Input.Email,
                    FirstName = Input.Voornaam, LastName = Input.Achternaam, ZipCode = Input.PostCode,
                    Location = new Point(zipCodeInformation.Latitude, zipCodeInformation.Longitude)
                };
                var emailExist = _context.User.Any(p => p.Email == Input.Email);
                if (emailExist)
                {
                    ModelState.AddModelError("EmailError", "Het e-mailadres is al in gebruik");
                }
                else
                {
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (_userManager.Users.Count() == 1)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Customer");
                    }

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new {area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl},
                            protocol: Request.Scheme);

                        using (MailMessage message = new MailMessage("plantjesbuurt@gmail.com", Input.Email))
                        {
                            message.Subject = "Account verificatie";
                            message.Body = "\nHey " + Input.Voornaam + " " + Input.Achternaam +
                                           ",\n\n Je kunt je account bijna gebruiken! Klik alleen nog even op de onderstaande link" +
                                           "om je email te bevesitgen!\n\n" + callbackUrl +
                                           "Groetjes,\n\n\nHet hele Plantjesbuurt Team!";
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

                            return LocalRedirect(ReturnUrl);
                        }
                        
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                // If we got this far, something failed, redisplay form
                return Page();
            }
            return LocalRedirect(ReturnUrl);
        }
    }
}