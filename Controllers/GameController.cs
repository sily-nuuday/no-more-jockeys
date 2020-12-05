using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NoMoreJockeys.Domain;
using NoMoreJockeys.Hubs;
using NoMoreJockeys.Logic;

namespace NoMoreJockeys.Controllers
{
    [Route("games")]
    public class GameController : ControllerBase
    {
        private readonly IGameClient _gameClient;

        private readonly Dictionary<string, Game> _games;

        public GameController(IGameClient gameClient)
        {
            _gameClient = gameClient;

            _games = new Dictionary<string, Game>();
        }

        [HttpGet("")]
        public IActionResult GetGames()
        {
            // TODO Subscribe caller to game list update

            return Ok(_games.Values.ToList());
        }

        [HttpPost("")]
        public IActionResult CreateGame(string playerName, int answerSeconds, int challengeSeconds)
        {
            Player admin = new Player(playerName);

            var game = new Game(admin, answerSeconds, challengeSeconds);
            _games.Add(game.Code, game);

            // TODO Subscribe caller to game update

            _gameClient.GameCreated(_games.Values.ToList());

            return Ok(admin.Code);
        }

        [HttpGet("{code}")]
        public IActionResult GetGame([FromRoute] string code)
        {
            if (!_games.ContainsKey(code))
            {
                return NotFound();
            }

            // TODO Subscribe caller to game updates

            return Ok(_games[code]);
        }

        [HttpPatch("{code}")]
        public IActionResult UpdateGame([FromRoute] string code, string adminCode, int answerSeconds, int challengeSeconds)
        {
            if (!_games.ContainsKey(code))
            {
                return NotFound();
            }

            var game = _games[code];

            if (game.Status != GameStatus.NotStarted)
            {
                return StatusCode(403, "Can only change game rules before start");
            }

            if (game.Players.First().Code != adminCode)
            {
                return Unauthorized();
            }

            game.AnswerSeconds = answerSeconds;
            game.ChallengeSeconds = challengeSeconds;

            _gameClient.GameUpdated(game);

            return Ok();
        }

        [HttpPatch("{code}/join")]
        public IActionResult JoinGame([FromRoute] string code, string playerName)
        {
            if (!_games.ContainsKey(code))
            {
                return NotFound();
            }

            var game = _games[code];

            if (game.Status != GameStatus.NotStarted)
            {
                return StatusCode(403, "Can only join game before start");
            }

            var player = new Player(playerName);
            game.Players.Add(player);

            // TODO Subscribe caller to game update

            _gameClient.GameUpdated(game);

            return Ok(player.Code);
        }

        [HttpPatch("{code}/start")]
        public IActionResult StartGame([FromRoute] string code, string adminCode)
        {
            if (!_games.ContainsKey(code))
            {
                return NotFound();
            }

            var game = _games[code];

            if (game.Status != GameStatus.NotStarted)
            {
                return StatusCode(403, "Can only start unstarted games");
            }

            if (game.Players.First().Code != adminCode)
            {
                return Unauthorized();
            }

            game.Status = GameStatus.NormalTurn;

            TimerStore.SetupAnswerTimeout(game);

            _gameClient.GameUpdated(game);

            return Ok();
        }

        [HttpPatch("{code}/answer")]
        public IActionResult SendAnswer([FromRoute] string code, string playerCode, string answer, string rule)
        {
            if (!_games.ContainsKey(code))
            {
                return NotFound();
            }

            var game = _games[code];

            if (game.CurrentPlayer.Code != playerCode)
            {
                return Unauthorized();
            }

            if (game.Status == GameStatus.NameAnother && rule != null)
            {
                return BadRequest("Adding rule not allowed when naming another");
            }

            game.Turns.Add(new Turn(game.Turns.Count, game.CurrentPlayer, game.Status, answer, rule));
            game.CurrentPlayer = game.FindNextPlayer();

            TimerStore.SetupAnswerTimeout(game);

            _gameClient.GameUpdated(game);

            return Ok();
        }

        [HttpPatch("{code}/name-another")]
        public IActionResult RequestNameAnother([FromRoute] string code, string playerCode, int turnId)
        {
            if (!_games.ContainsKey(code))
            {
                return NotFound();
            }

            var game = _games[code];

            if (game.Turns.Last().TurnId != turnId)
            {
                return StatusCode(403, "Cannot request name another for rule other than latest");
            }

            if (game.Turns.Last().TurnType == GameStatus.NameAnother)
            {
                return StatusCode(403, "Name another can only be requested once per answer");
            }

            if (game.Status == GameStatus.NameAnother)
            {
                return Conflict("Name another has already been requested");
            }

            Player player = game.FindActivePlayer(playerCode);
            if (player == null || player == game.CurrentPlayer)
            {
                return Unauthorized();
            }

            game.Status = GameStatus.NameAnother;
            game.CurrentPlayer = game.FindPreviousPlayer();

            TimerStore.SetupAnswerTimeout(game);

            _gameClient.GameUpdated(game);

            return Ok();
        }

        [HttpPatch("{code}/challenge")]
        public IActionResult Challenge([FromRoute] string code, string playerCode, int turnId)
        {
            if (!_games.ContainsKey(code))
            {
                return NotFound();
            }

            var game = _games[code];

            if (game.Turns.Last().TurnId != turnId)
            {
                return StatusCode(403, "Cannot challenge answer other than latest");
            }

            if (game.Turns.Last().TurnType == GameStatus.Challenge)
            {
                return StatusCode(403, "Challenge can only be posited once per answer");
            }

            if (game.Status == GameStatus.NameAnother)
            {
                return StatusCode(403, "Name another answer cannot be challenged");
            }

            Player player = game.FindActivePlayer(playerCode);
            Player challengedPlayer = game.FindPreviousPlayer();
            if (player == null || player == challengedPlayer)
            {
                return Unauthorized();
            }

            game.Status = GameStatus.Challenge;
            game.CurrentPlayer = challengedPlayer;
            game.ChallengingPlayer = player;

            TimerStore.SetupChallengeTimeout(game);

            _gameClient.GameUpdated(game);

            return Ok();
        }

        [HttpPatch("{code}/challenge/vote")]
        public IActionResult Vote([FromRoute] string code, string playerCode, bool vote)
        {
            if (!_games.ContainsKey(code))
            {
                return NotFound();
            }

            var game = _games[code];

            if (game.Status != GameStatus.Challenge)
            {
                return StatusCode(403, "Can only vote if challenge is ongoing");
            }

            Player player = game.FindActivePlayer(playerCode);
            if (player == null || player == game.CurrentPlayer)
            {
                return Unauthorized();
            }

            if (game.Votes.Exists(v => v.Player == player))
            {
                return Conflict("Cannot vote twice on one challenge");
            }

            game.Votes.Add(new Vote(player, vote));

            // Check if everyone but challenged player has voted
            if (game.Votes.Count == game.CountActivePlayers() - 1)
            {
                game.ResolveChallenge();

                TimerStore.SetupAnswerTimeout(game);
            }

            _gameClient.GameUpdated(game);

            return Ok();
        }
    }
}
