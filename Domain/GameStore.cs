using System.Collections.Generic;
using System.Linq;

namespace NoMoreJockeys.Domain
{
    public static class GameStore
    {
        private static readonly Dictionary<string, Game> Games = new Dictionary<string, Game>();

        public static List<GameOverview> GetGames()
        {
            return Games.Values.Select(e => new GameOverview(e)).ToList();
        }

        public static Game GetGame(string code)
        {
            if (!Games.ContainsKey(code))
            {
                return null;
            }

            return Games[code];
        }

        public static Game AddGame(Player admin, int answerSeconds, int challengeSeconds)
        {
            var game = new Game(admin, answerSeconds, challengeSeconds);
            Games.Add(game.Code, game);

            return game;
        }
    }
}
