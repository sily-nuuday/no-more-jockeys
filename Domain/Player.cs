using System;
using Newtonsoft.Json;

namespace NoMoreJockeys.Domain
{
    public class Player
    {
        [JsonIgnore]
        public string Code { get; set; }
        public string Name { get; set; }
        public int ChallengesRemaining { get; set; }

        public Player(string name)
        {
            Code = Guid.NewGuid().ToString();
            Name = name;
            ChallengesRemaining = 3;
        }
    }
}
