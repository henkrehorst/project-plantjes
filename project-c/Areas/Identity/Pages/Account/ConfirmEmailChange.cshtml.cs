using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using project_c.Models.Users;

namespace project_c.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailChangeModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DataContext _dataContext;

        public ConfirmEmailChangeModel(UserManager<User> userManager, SignInManager<User> signInManager, DataContext dataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dataContext = dataContext;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            // workaround check recovery code
            if (user.EmailRecoveryCode != code)
            {
                StatusMessage = "Er is een fout opgetreden, probeer het opnieuw";
                return Page();
            }
            
            // added workaround
            if (_dataContext.User.Count(u => u.Email == email) != 0)
            {
                StatusMessage = "Er is een fout opgetreden, probeer het opnieuw";
                return Page();
            }

            user.Email = email;
            user.UserName = email;
            user.UserName = email;
            user.NormalizedEmail = email.ToUpper();
            user.NormalizedUserName = email.ToUpper();
            _dataContext.Update(user);
            await _dataContext.SaveChangesAsync();
            
            
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Uw email is succesvol gewijzigd!";
            return Page();
        }
    }
}
