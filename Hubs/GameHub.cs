using Microsoft.AspNetCore.SignalR;

namespace NoMoreJockeys.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
    }
}
