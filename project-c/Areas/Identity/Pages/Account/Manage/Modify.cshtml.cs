using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using project_c.Models.Users;
using project_c.Services;

namespace project_c.Areas.Identity.Pages.Account.Manage
{

    // IN PLAATS VAN Console.WriteLine(), GEBRUIK System.Diagnostics.Debug.WriteLine(). CTRL+ALT+O
    public class ModifyModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly DataContext _context;
        private readonly UploadService _uploadService;
        

        public ModifyModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<ChangePasswordModel> logger,
            IEmailSender emailSender,
            DataContext context,
            UploadService uploadService)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _uploadService = uploadService;
        }
        [Display(Name = "Gebruikersnaam")]
        public string Username { get; set; }

        [Display(Name = "Voornaam")]
        public string FirstName { get; set; }

        [Display(Name = "Achternaam")]
        public string LastName { get; set; }

        [Display(Name = "Postcode")]
        public string Zipcode { get; set; }
        
        [Display(Name = "Huidig Emailadres")]
        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData] 
        public string StatusMessage { get; set; }

        [BindProperty]
        public EmailInputModel EmailInput { get; set; }
        public UserData userData { get; set; }
        public User usr { get; set; }
        public string usrid { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        [DataType(DataType.Url)]
        [Display(Name = "Profielfoto")]
        public string avatar { get; set; }

        //User code.
        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            userData = _context.UserData.Where(u => u.UserId == user.Id).Single();
            usr = user;
            lat = userData.Lat;
            lng = userData.Lng;
            usrid = userData.UserId;
            avatar = userData.Avatar;
            FirstName = userData.FirstName;
            LastName = userData.LastName;
            Zipcode = userData.ZipCode;
            Username = userName;
            Email = email;

        }

        public async Task<IActionResult> OnPostAsync(UserData userData, double lat, double lng, IFormCollection form)
        {
            var user = await _userManager.GetUserAsync(User);
            userData = _context.UserData.Where(u => u.UserId == user.Id).Single();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            user.UserName = form["Username"].ToString();
            userData.FirstName = form["FirstName"].ToString();
            userData.LastName = form["LastName"].ToString();
            userData.ZipCode = form["Zipcode"].ToString();
            userData.Lat = lat;
            userData.Lng = lng;
            try
            {
                IFormFile image = form.Files.GetFile("ImageUpload");
                if (image != null)
                {
                    userData.Avatar = await _uploadService.UploadImage(image);
                }
            }
            catch
            {
                return Content("Error - Probeer het opnieuw.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _context.Update(userData);
            _context.SaveChanges();
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        [HttpPost]
        public ActionResult UploadAvatar()
        {
            return RedirectToPage();
        }

        //Email code.
        public class EmailInputModel
        {
            [EmailAddress]
            [Display(Name = "Nieuwe email")]
            public string NewEmail { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (EmailInput.NewEmail != email && EmailInput.NewEmail != null)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, EmailInput.NewEmail);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { userId = userId, email = EmailInput.NewEmail, code = code },
                    protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(
                    EmailInput.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                StatusMessage = "Confirmation link to change email sent. Please check your email.";
                return RedirectToPage();
            }

            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }

        //Avatar.
        public async Task<IActionResult> SetAvatar(IFormCollection form)
        {
            var user = _userManager.GetUserAsync(User);
            var userdata = _context.UserData.FirstOrDefault(ud => ud.UserId == "" + user.Id);
            try
            {
                if (ModelState.IsValid)
                {
                    IFormFile image = form.Files.GetFile("AvatarUpload");
                    userdata.Avatar = await _uploadService.UploadImage(image);
                    _context.Update(userdata);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(System.Index));
            }
            catch
            {
                return Content("Error - Probeer het opnieuw.");
            }
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}