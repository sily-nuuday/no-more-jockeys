using System;
using System.Collections.Generic;
using System.Linq;

namespace NoMoreJockeys.Domain
{
    public class Game
    {
        public string Code { get; set; }
        public int AnswerSeconds { get; set; }
        public int ChallengeSeconds { get; set; }
        public GameStatus Status { get; set; }
        public List<Player> Players { get; set; }
        public List<Turn> Turns { get; set; }
        public Player CurrentPlayer { get; set; }
        public Player ChallengingPlayer { get; set; }
        public List<Vote> Votes { get; set; }

        public Game(Player admin, int answerSeconds, int challengeSeconds)
        {
            Code = Guid.NewGuid().ToString();
            AnswerSeconds = answerSeconds;
            ChallengeSeconds = challengeSeconds;
            Status = GameStatus.NotStarted;
            Players = new List<Player> { admin };
            Turns = new List<Turn>();
            CurrentPlayer = admin;
            Votes = new List<Vote>();
        }

        public Player FindPreviousPlayer()
        {
            return Turns.Last().Player;
        }

        public Player FindActivePlayer(string code)
        {
            return Players.FirstOrDefault(p => p.ChallengesRemaining > 0 && p.ConnectionId == code);
        }

        public Player FindNextPlayer()
        {
            int currentPlayerIndex = Players.IndexOf(CurrentPlayer);
            int index = currentPlayerIndex;
            Player nextPlayer = null;
            while (nextPlayer == null)
            {
                index++;
                if (index == Players.Count)
                {
                    index = 0;
                }

                if (Players[index].ChallengesRemaining > 0)
                {
                    nextPlayer = Players[index];
                }
            }

            return nextPlayer;
        }

        public int CountActivePlayers()
        {
            return Players.Count(p => p.ChallengesRemaining > 0);
        }

        public void ResolveChallenge()
        {
            int yeas = Votes.Count(v => v.Choice);
            int nays = Votes.Count - yeas;

            if (yeas > nays)
            {
                CurrentPlayer.ChallengesRemaining = 0;
            }
            else
            {
                ChallengingPlayer.ChallengesRemaining--;
            }

            Status = GameStatus.NormalTurn;
            CurrentPlayer = FindNextPlayer();
            ChallengingPlayer = null;
            Votes = new List<Vote>();

            CheckForGameEnd();
        }

        public void CheckForGameEnd()
        {
            if (CountActivePlayers() < 2)
            {
                Status = GameStatus.Completed;
            }
        }
    }
}