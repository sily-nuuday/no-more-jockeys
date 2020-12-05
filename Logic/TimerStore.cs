using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NoMoreJockeys.Domain;

namespace NoMoreJockeys.Logic
{
    public static class TimerStore
    {
        private static readonly Dictionary<string, CancellationTokenSource> GameTasks = new Dictionary<string, CancellationTokenSource>();

        public static void SetupAnswerTimeout(Game game)
        {
            SetupTimeout(game, game.AnswerSeconds, () =>
            {
                game.CurrentPlayer.ChallengesRemaining = 0;
                game.Status = GameStatus.NormalTurn;
                game.CurrentPlayer = game.FindNextPlayer();

                game.CheckForGameEnd();

                SetupAnswerTimeout(game);
            });
        }

        public static void SetupChallengeTimeout(Game game)
        {
            SetupTimeout(game, game.ChallengeSeconds, () =>
            {
                game.ResolveChallenge();

                SetupChallengeTimeout(game);
            });
        }

        private static void SetupTimeout(Game game, int seconds, Action callback)
        {
            if (GameTasks.ContainsKey(game.Code))
            {
                GameTasks[game.Code].Cancel();
                GameTasks.Remove(game.Code);
            }

            if (game.Status == GameStatus.Completed || seconds == 0)
            {
                return;
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            Task.Delay(seconds).ContinueWith(t =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                callback();
            });

            GameTasks.Add(game.Code, cancellationTokenSource);
        }
    }
}
