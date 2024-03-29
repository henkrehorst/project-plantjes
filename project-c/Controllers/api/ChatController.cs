﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using project_c.Models.Users;
using Message = project_c.Models.Chat.Message;

namespace project_c.Controllers.api
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class ChatController: Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public ChatController(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        public class Chat
        {
            public string Avatar { get; set; }
            public string FirstName { get; set; }
            public DateTime LastMessage { get; set; }
            public string UserId { get; set; }
            public int UnreadMessages { get; set; }
        }

        public class Chatroom
        {
            public string Avatar { get; set; }
            public string FirstName { get; set; }
            public List<Message> Messages { get; set; }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Chats()
        {
            //get id of current user
            var userId = _userManager.GetUserId(User);

            //get chats of user
            var chats = await _context.Messages
                .Join(_context.User, m => (m.UserId == userId ? m.ReceivedUserId : m.UserId), u => u.Id,
                    (m, u) => new {m, u})
                .Where(@t => @t.m.UserId == userId | @t.m.ReceivedUserId == userId)
                .GroupBy(t => new {t.u.Avatar, t.u.FirstName, t.u.Id })
                .Select(@t => new Chat {Avatar = @t.Key.Avatar, 
                    FirstName = @t.Key.FirstName, 
                    UserId = @t.Key.Id,
                    LastMessage = t.Max(h => h.m.When),
                    UnreadMessages = t.Sum(i => !i.m.IsRead & i.m.ReceivedUserId == userId ? 1 : 0 )
                })
                .OrderByDescending(t => t.LastMessage)
                .ToListAsync();
            
            if (chats.Count == 0) return NoContent();

            return Ok(chats);
        }

        [HttpGet]
        [Authorize]
        [Route("/api/[controller]/messages/{receiverId}")]
        public ActionResult Messages(string receiverId)
        {
            //get id of current user
            var userId = _userManager.GetUserId(User);
            
            //get other user
            var other = _context.User.First(u => u.Id == receiverId);

            if (other == null) return NotFound();
            
            var messages =  _context.Messages.
                Where(m => m.UserId == userId & m.ReceivedUserId == receiverId || 
                           m.UserId == receiverId & m.ReceivedUserId == userId)
                .Select(m => new Message
                {
                    User = null, 
                    IsRead = m.IsRead, 
                    MessageId = m.MessageId,
                    ReceivedUserId = m.ReceivedUserId, 
                    Text = m.Text, 
                    UserId = m.UserId, 
                    When = m.When
                })
                .OrderByDescending(m => m.When)
                .Take(30)
                .ToList();

            return Ok(new Chatroom{Avatar = other.Avatar, FirstName = other.FirstName, Messages = messages});
        }
    }
}