using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using project_c.Helpers;
using project_c.Models.Users;
using project_c.Services;

namespace project_c.Areas.Identity.Pages.Account.Manage
{
    public class ChangeProfielModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _dataContext;
        private readonly UploadService _uploadService;

        public ChangeProfielModel(
            UserManager<User> userManager,
            DataContext dataContext,
            UploadService uploadService)
        {
            _userManager = userManager;
            _dataContext = dataContext;
            _uploadService = uploadService;
        }

        [BindProperty] public InputModel Input { get; set; }

        [TempData] public string StatusMessage { get; set; }

        public class InputModel
        {
            [StringLength(100, 
                ErrorMessage = "Uw bio moet minimaal 5 tekens bevatten en mag maximaal 100 tekens bevatten!", 
                MinimumLength = 5)]
            public string Bio { get; set; }
            
            [DataType(DataType.Upload)]
            [MaxFileSize(2* 1024 * 1024)]
            [AllowedExtensions(new string[] { ".jpg" })]
            public IFormFile Avatar { get; set; }
            
            [DataType(DataType.Upload)]
            [MaxFileSize(2* 1024 * 1024)]
            [AllowedExtensions(new string[] { ".jpg" })]
            public IFormFile Banner { get; set; }
            
            public string BannerLocation { get; set; }
            public string AvatarLocation { get; set; }
            
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            
            Input = new InputModel();
            //field bio field
            if (user.Bio != null)
            {
                Input.Bio = user.Bio;
                Input.AvatarLocation = user.Avatar;
                Input.BannerLocation = user.ProfileBanner;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (!ModelState.IsValid)
            {
                Input.Bio = user.Bio;
                Input.AvatarLocation = user.Avatar;
                Input.BannerLocation = user.ProfileBanner;
                return Page();
            }
            
            //update bio
            user.Bio = Input.Bio;
            
            //update avatar if is uploaded
            if (Input.Avatar != null)
            {
                user.Avatar = await _uploadService.UploadImage(Input.Avatar);
            }
            
            //update banner if is uploaded
            if (Input.Banner != null)
            {
                user.ProfileBanner = await _uploadService.UploadImage(Input.Banner);
            }

            _dataContext.Update(user);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("MijnPlanten","Plants");
        }
    }
}