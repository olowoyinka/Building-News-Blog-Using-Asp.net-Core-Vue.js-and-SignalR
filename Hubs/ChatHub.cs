using System.Threading.Tasks;
using NewsBlog.Data;
using Microsoft.AspNetCore.SignalR;
using NewsData.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace NewsBlog.Hubs
{   
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(int ArticleID, string Name, string Email, string Message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", ArticleID, Name, Email, Message);

            var model = new Comment()
            {
                ArticleID = ArticleID,
                Name = Name,
                Email = Email,
                Message = Message
            };

            AddComment(model);
        }

        public Task SendMessageToGroups(string message)
        {
            List<string> groups = new List<string>() { "SignalR Users" };
            return Clients.Groups(groups).SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public Task SendMessageToCaller(int ArticleID, string Name, string Email, string Message)
        {
            var model = new Comment()
            {
                ArticleID = ArticleID,
                Name = Name,
                Email = Email,
                Message = Message
            };

            AddComment(model);

            return Clients.Caller.SendAsync("ReceiveMessage", ArticleID, Name, Email, Message);
        }

        private void AddComment([FromBody]Comment model)
        {

            _context.Comments.Add(model);
            _context.SaveChanges();

        }

        
    }
    
}
