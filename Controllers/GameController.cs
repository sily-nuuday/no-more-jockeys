using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NoMoreJockeys.Domain;
using NoMoreJockeys.Logic;

namespace NoMoreJockeys.Controllers
{
    [Route("games")]
    public class GameController : ControllerBase
    {
        [HttpGet("")]
        public IActionResult GetGames()
        {
            // TODO Subscribe caller to game list update

            return Ok(GameStore.GetGames());
        }

        [HttpPost("")]
        public IActionResult CreateGame(string playerName, int answerSeconds, int challengeSeconds)
        {
            Player admin = new Player(playerName);

            GameStore.AddGame(admin, answerSeconds, challengeSeconds);

            // TODO Subscribe caller to game update

            // TODO _gameClient.GameCreated(_games.Values.ToList());

            return Ok(admin.Code);
        }

        [HttpGet("{code}")]
        public IActionResult GetGame([FromRoute] string code)
        {
            var game = GameStore.GetGame(code);
            if (game == null)
            {
                return NotFound();
            }

            // TODO Subscribe caller to game updates

            return Ok(game);
        }

        [HttpPatch("{code}")]
        public IActionResult UpdateGame([FromRoute] string code, string adminCode, int answerSeconds, int challengeSeconds)
        {
            var game = GameStore.GetGame(code);
            if (game == null)
            {
                return NotFound();
            }
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

            // TODO _gameClient.GameUpdated(game);

            return Ok();
        }

        [HttpPatch("{code}/join")]
        public IActionResult JoinGame([FromRoute] string code, string playerName)
        {
            var game = GameStore.GetGame(code);
            if (game == null)
            {
                return NotFound();
            }
            if (game.Status != GameStatus.NotStarted)
            {
                return StatusCode(403, "Can only join game before start");
            }

            var player = new Player(playerName);
            game.Players.Add(player);

            // TODO Subscribe caller to game update

            // TODO _gameClient.GameUpdated(game);

            return Ok(player.Code);
        }

        [HttpPatch("{code}/start")]
        public IActionResult StartGame([FromRoute] string code, string adminCode)
        {
            var game = GameStore.GetGame(code);
            if (game == null)
            {
                return NotFound();
            }
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

            // TODO _gameClient.GameUpdated(game);

            return Ok();
        }

        [HttpPatch("{code}/answer")]
        public IActionResult SendAnswer([FromRoute] string code, string playerCode, string answer, string rule)
        {
            var game = GameStore.GetGame(code);
            if (game == null)
            {
                return NotFound();
            }
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

            // TODO _gameClient.GameUpdated(game);

            return Ok();
        }

        [HttpPatch("{code}/name-another")]
        public IActionResult RequestNameAnother([FromRoute] string code, string playerCode, int turnId)
        {
            var game = GameStore.GetGame(code);
            if (game == null)
            {
                return NotFound();
            }
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

            // TODO _gameClient.GameUpdated(game);

            return Ok();
        }

        [HttpPatch("{code}/challenge")]
        public IActionResult Challenge([FromRoute] string code, string playerCode, int turnId)
        {
            var game = GameStore.GetGame(code);
            if (game == null)
            {
                return NotFound();
            }
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

            // TODO _gameClient.GameUpdated(game);

            return Ok();
        }

        [HttpPatch("{code}/challenge/vote")]
        public IActionResult Vote([FromRoute] string code, string playerCode, bool vote)
        {
            var game = GameStore.GetGame(code);
            if (game == null)
            {
                return NotFound();
            }
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

            // TODO _gameClient.GameUpdated(game);

            return Ok();
        }
    }
}
