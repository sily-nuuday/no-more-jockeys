using System.Collections.Generic;
using System.Threading.Tasks;
using NoMoreJockeys.Domain;

namespace NoMoreJockeys.Hubs
{
    public interface IGameClient
    {
        Task GameListUpdated(List<GameOverview> games);
        Task GameUpdated(Game game);
    }
}
