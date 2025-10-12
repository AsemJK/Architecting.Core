using Microsoft.AspNetCore.SignalR;

namespace myapi_minimals.SignalR
{
    public class UpdateHub : Hub
    {
        public async Task SendUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveUpdate", message);
        }
    }
}
