using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
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
using NetTopologySuite.Geometries;
using project_c.Models.Users;
using project_c.Services.GeoRegister.Service;
using project_c.Services;

namespace project_c.Areas.Identity.Pages.Account.Manage
{
    public class ModifyModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly DataContext _context;
        private readonly ZipCodeService _zipCodeService;
        private readonly UploadService _uploadService;
        

        public ModifyModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<ChangePasswordModel> logger,
            IEmailSender emailSender,
            DataContext context,
            ZipCodeService zipCodeService,
            UploadService uploadService)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _zipCodeService = zipCodeService;
            _uploadService = uploadService;
        }

        public class InputModel
        {
            [Display(Name = "Voornaam")]
            [Required(ErrorMessage = "Voer een achternaam in")]
            public string FirstName { get; set; }

            [Display(Name = "Achternaam")]
            [Required(ErrorMessage = "Voer een achternaam in")]
            public string LastName { get; set; }
            
            [Display(Name = "Postcode")]
            public string Zipcode { get; set; }
            
            [Display(Name = "Huidig Emailadres")]
            public string Email { get; set; }
            
            [EmailAddress]
            [Display(Name = "Nieuwe email")]
            [Required(ErrorMessage = "Voer een email adres in")]
            public string NewEmail { get; set; }
            
            public bool IsEmailConfirmed { get; set; }
            
            public string ProspectForm { get; set; }
        }

        [TempData] 
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        //User code.
        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            Input = new InputModel();
            Input.FirstName = user.FirstName;
            Input.LastName = user.LastName;
            Input.Zipcode = user.ZipCode;
            Input.Email = email;
            Input.IsEmailConfirmed = user.EmailConfirmed;
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

        public async Task<IActionResult> OnPostAsync()
        {
            switch (Input.ProspectForm)
            {
                case "PersonInformation":
                    return await ChangePersonalInformation();
                case "EmailConfirmed":
                    return await OnPostSendVerificationEmailAsync();
                case "NewEmail":
                    return await OnPostChangeEmailAsync();
                default:
                    return RedirectToPage();
            }
        }

        public async Task<IActionResult> ChangePersonalInformation()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            //get zipcode information
            var zipCodeInformation = await _zipCodeService.GetZipCodeInformation(Input.Zipcode);
            //check zipcode is valid
            if (zipCodeInformation == null)
            {
                ModelState.AddModelError("Input.Zipcode", "Deze postcode is ongeldig, probeer een andere!");
                
            }
            
            if (ModelState.ContainsKey("Input.NewEmail"))
                ModelState.Remove("Input.NewEmail");
            
            if (ModelState.IsValid)
            {
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.ZipCode = Input.Zipcode;
                user.Location = new Point(zipCodeInformation.Latitude, zipCodeInformation.Longitude);

                await _signInManager.RefreshSignInAsync(user);
                // _context.Add(user);
                _context.SaveChanges();
                StatusMessage = "Uw profiel is aangepast.";
                return RedirectToPage();
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
            
            // ignore errors of other forms 
            if (ModelState.ContainsKey("Input.Firstname"))
                ModelState.Remove("Input.Firstname");
            if (ModelState.ContainsKey("Input.Lastname"))
                ModelState.Remove("Input.Lastname");
            if (ModelState.ContainsKey("Input.Zipcode"))
                ModelState.Remove("Input.Zipcode");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email && Input.NewEmail != null)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                
                // link recovery code
                user.EmailRecoveryCode = code;
                _context.Update(user);
                await _context.SaveChangesAsync();
                
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme); 
                
                using (MailMessage message = new MailMessage("plantjesbuurt@gmail.com", Input.NewEmail))
                {
                    message.Subject = "Bevestig uw nieuwe email";
                    message.Body = "\nHey " + user.FirstName + " " + Input.LastName +
                                   ",\n\n Je kunt je nieuwe email bijna gebruiken! \n\n" +
                                   $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Klik alleen hier nog even om je email te bevestigen!</a><br/>" + " \n" +
                                   "Groetjes,\n\n\nHet hele Plantjesbuurt Team!";
                    message.IsBodyHtml = true;

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
                StatusMessage = "Bevestigings link verstuurd naar het nieuwe email. Bekijk uw email account a.u.b.";
                return RedirectToPage();
            }

            StatusMessage = "Uw email is niet veranderd.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
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
            using (MailMessage message = new MailMessage("plantjesbuurt@gmail.com", Input.Email))
            {
                message.Subject = "Account verificatie";
                message.Body = "\nHey " + user.FirstName + " " + user.FirstName +
                               ",\n\n Je kunt je account bijna gebruiken! Klik alleen nog even op de onderstaande link" +
                               "om je email te bevesitgen!\n\n" + callbackUrl + " \n" + 
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
            }

            StatusMessage = "Bevestigingsemail verstuurd. Bekijk uw email a.u.b.";
            return RedirectToPage();
        }
    }
}