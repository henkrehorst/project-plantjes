using Microsoft.AspNetCore.SignalR;
using project_c.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using project_c.Models.Users;

namespace project_c.Hubs
{
    public class ChatHub : Hub
    {
        private DataContext _dataContext;
        private readonly UserManager<User> _userManager;
        
        public ChatHub(DataContext dataContext, UserManager<User> userManager)
        {
            this._dataContext = dataContext;
            this._userManager = userManager;
        }
        
        public async Task SendMessage(string receiver, string text)
        {
            var sender =  _userManager.GetUserId(Context.User);
            var message = new Message(){ReceivedUserId = receiver, 
                Text = text, 
                UserId = sender,
                When = DateTime.Now
            };
             _dataContext.Messages.Add(message);
             _dataContext.SaveChanges();

            //remove user data from object
            message.User = null;

            await Clients.User(receiver).SendAsync("receiveMessage", JsonSerializer.Serialize<Message>(message));
        }

        public async Task Hello(string hello) =>
            await Clients.All.SendAsync("hello", hello);

        //get number of unread messages
        public async Task CountUnreadMessages()
        {
            var receivedId = _userManager.GetUserId(Context.User);
            var count = await _dataContext.Messages.Where(m => m.ReceivedUserId == receivedId & m.IsRead == false).CountAsync();
            await Clients.User(receivedId).SendAsync("unreadMessageCount", count);
        }
        
        //set read messages
        public async Task ReadMessages(string userId)
        {
            var receivedId = _userManager.GetUserId(Context.User);
            //TODO-later looking for a code first solution
            await _dataContext.Database
                .ExecuteSqlRawAsync(
                    $"UPDATE \"Messages\" SET \"IsRead\" = true where \"ReceivedUserId\" = '{receivedId}' and \"UserId\" = '{userId}'");
            
            var count = await _dataContext.Messages.Where(m => m.ReceivedUserId == receivedId & m.IsRead == false).CountAsync();
            await Clients.User(receivedId).SendAsync("unreadMessageCount", count);
        }
    }
}
