using Microsoft.AspNetCore.SignalR;
using project_c.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            var sender = await _userManager.GetUserAsync(Context.User);
            var message = new Message(){ReceivedUserId = receiver, Text = text, UserId = sender.Id};
            _dataContext.Messages.Add(message);
            await _dataContext.SaveChangesAsync();

            //remove user data from object
            message.User = null;

            await Clients.User(receiver).SendAsync("receiveMessage", JsonSerializer.Serialize<Message>(message));
        }

        public async Task Hello(string hello) =>
            await Clients.All.SendAsync("hello", hello);

        public async Task SendUserMessage(string userId, string message) =>
            await Clients.User(userId).SendAsync("hello",message);
    }
}
