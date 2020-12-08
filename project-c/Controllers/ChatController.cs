using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using project_c.Models.Chat;
using Microsoft.AspNetCore.Identity;
using project_c.Models.Users;
using System.Security.Claims;

namespace project_c.Controllers
{
    public class ChatController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;
        public ChatController(UserManager<User> usr, DataContext context)
        {
            _userManager = usr;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        //GET: ChatController/NewChat
        [Authorize]
        public ActionResult Create()
        {
            return View("NewChat");
        }

        //Post: ChatController/NewChat
        [Authorize]
        public async Task<ActionResult> CreateNew(IFormCollection form, User otheruser)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var msg = form["Message"].ToString();
            var usr = form["User"];
            DateTime created = DateTime.Today;
            try
            {
                Chat chat = new Chat();
                chat.Created = created;
                chat.ChatData = new ChatData();
                chat.ChatData.Users.Add(user);
                chat.ChatData.Users.Add(_context.User.Where(a => a.Id == otheruser.Id).Single());
                if (!ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Index));
                }
                _context.Chats.Add(chat);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Content("Er was een fout. Probeer het opnieuw.");
            }
        }
    }
}