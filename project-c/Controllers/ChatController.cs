using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;

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

        public async void Send(int receiver)
        {
            var msg = form["Message"].ToString();
        }
    }
}