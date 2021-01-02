using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_c.Models;
using project_c.Models.Chat;
using project_c.Models.Users;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace project_c.Controllers
{
    public class ChatController : Controller
    {
        public readonly DataContext _context;

        public readonly UserManager<User> _userManager;

        public ChatController(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ViewResult> Index()
        {
            var users = await _context.User.ToListAsync();

            return View(users);
        }


        [Authorize]
        public async Task<IActionResult> ChatIndex(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUserName = currentUser.UserName;
            var messages = await _context.Messages.
                Where(m => m.UserId == currentUser.Id & m.ReceivedUserId == id || 
                           m.UserId == id & m.ReceivedUserId == currentUser.Id)
                .ToListAsync();

            ViewData["receiverId"] = id;
            
            return View(messages);
        }

        public async Task<IActionResult> Create(Message message)
        {
            if (ModelState.IsValid)
            {
                var sender = await _userManager.GetUserAsync(User);
                message.UserId = sender.Id;
                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return Error();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}