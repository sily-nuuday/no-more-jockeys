namespace NoMoreJockeys.Domain
{
    public class Vote
    {
        public Player Player { get; set; }
        public bool Choice { get; set; }

        public Vote(Player player, bool choice)
        {
            Player = player;
            Choice = choice;
        }
    }
}
