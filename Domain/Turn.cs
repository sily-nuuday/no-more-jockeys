namespace NoMoreJockeys.Domain
{
    public class Turn
    {
        public int TurnId { get; set; }
        public Player Player { get; set; }
        public GameStatus TurnType { get; set; }
        public string Answer { get; set; }
        public string Rule { get; set; }

        public Turn(int turnId, Player player, GameStatus turnType, string answer, string rule)
        {
            TurnId = turnId;
            Player = player;
            TurnType = turnType;
            Answer = answer;
            Rule = rule;
        }
    }
}
