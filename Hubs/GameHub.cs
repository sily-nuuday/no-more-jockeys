using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NoMoreJockeys.Domain;

namespace NoMoreJockeys.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
        private static readonly string GameList = "gameList";
        
        public async Task RetrieveGameList()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GameList);
            await Clients.Caller.GameListUpdated(GameStore.GetGames());
        }
        
        public async Task UnsubscribeGameList()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GameList);
        }

        public async Task CreateGame(string playerName, int answerSeconds, int challengeSeconds)
        {
            Player admin = new Player(Context.ConnectionId, playerName);
            Game game = GameStore.AddGame(admin, answerSeconds, challengeSeconds);

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Code);
            await Clients.Caller.GameUpdated(game);
            await Clients.Group(GameList).GameListUpdated(GameStore.GetGames());
        }

        public async Task JoinGame(string playerName, string code)
        {
            Game game = GameStore.GetGame(code);

            Player player = new Player(Context.ConnectionId, playerName);
            game.Players.Add(player);

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Code);
            await Clients.Group(game.Code).GameUpdated(game);
            await Clients.Group("gameList").GameListUpdated(GameStore.GetGames());
        }

        public async Task LeaveGame(string gameCode)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameCode);

            Game game = GameStore.GetGame(gameCode);
            var player = game.Players.FirstOrDefault(e => e.ConnectionId == Context.ConnectionId);
            if (player == null || game.Status == GameStatus.Completed)
            {
                return;
            }
            if (game.Status == GameStatus.NotStarted)
            {
                game.Players.Remove(player);
                if (game.Players.Count == 0)
                {
                    GameStore.RemoveGame(gameCode);
                    await Clients.Group(GameList).GameListUpdated(GameStore.GetGames());
                }

                return;
            }

            player.ChallengesRemaining = 0;
            if (game.Players.Count == 1)
            {
                game.Status = GameStatus.Completed;
            }
        }
    }
}
