using System;
using Newtonsoft.Json;

namespace NoMoreJockeys.Domain
{
    public class Player
    {
        [JsonIgnore]
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public int ChallengesRemaining { get; set; }

        public Player(string connectionId, string name)
        {
            ConnectionId = connectionId;
            Name = name;
            ChallengesRemaining = 3;
        }
    }
}
