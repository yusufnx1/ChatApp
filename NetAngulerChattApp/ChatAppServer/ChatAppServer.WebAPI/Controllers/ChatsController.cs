using ChatAppServer.WebAPI.Context;
using ChatAppServer.WebAPI.Dtos;
using ChatAppServer.WebAPI.Hubs;
using ChatAppServer.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class ChatsController(ApplicationDbContext context,IHubContext<ChatHub> hubContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            List<User> users = await context.Users.OrderBy(x => x.Name).ToListAsync();
            return Ok(users);
        }
        [HttpGet]
        public async Task<IActionResult> GetChats(Guid userID, Guid toUserId, CancellationToken cancellationToken)
        {
            List<Chatt> chats = await context.Chatts.Where
                (x => x.UserId == userID && x.ToUserId == toUserId ||
                 x.ToUserId == userID && x.UserId == toUserId)
                .OrderBy(x => x.Date).ToListAsync();

            return Ok(chats);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageDto request, CancellationToken cancellationToken)
        {
            Chatt chat = new()
            {
                UserId = request.UserId,
                ToUserId = request.ToUserId,
                Message = request.Message,
                Date = DateTime.Now,
            };

            await context.AddAsync(chat, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            string connectionId = ChatHub.Users.First(p => p.Value == chat.ToUserId).Key;


            await hubContext.Clients.Client(connectionId).SendAsync("Message",chat);

            return Ok(chat);
        }
    }
}
