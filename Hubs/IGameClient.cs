using System.Collections.Generic;
using System.Threading.Tasks;
using NoMoreJockeys.Domain;

namespace NoMoreJockeys.Hubs
{
    public interface IGameClient
    {
        Task GameCreated(List<Game> games);
        Task GameUpdated(Game game);
    }
}
