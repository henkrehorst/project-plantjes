using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using project_c.Models.Chat;

namespace project_c.Controllers
{
    public class ChatController : Controller
    {
        private readonly DataContext _context;

        public ChatController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CreateChat(IFormCollection form, int receiver)
        {
            var msg = form["Message"].ToString();
            DateTime created = DateTime.Today;
            try
            {
                Chat chat = new Chat();
                if (ModelState.IsValid)
                {
                    chat.Created = created;
                    chat.ChatData = new ChatData();
                    //chat.ChatData.Users.Add();
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Content("Er was een fout. Probeer het opnieuw.");
            }
        }
    }
}