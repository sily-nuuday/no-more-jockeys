using System.Linq;

namespace NoMoreJockeys.Domain
{
    public class GameOverview
    {
        public string Code { get; set; }
        public string AdminName { get; set; }
        public int PlayerCount { get; set; }
        public GameStatus Status { get; set; }

        public GameOverview(Game game)
        {
            Code = game.Code;
            AdminName = game.Players.First().Name;
            PlayerCount = game.Players.Count;
            Status = game.Status;
        }
    }
}
