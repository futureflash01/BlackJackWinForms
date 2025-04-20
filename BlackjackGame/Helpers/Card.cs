namespace BlackjackGame.Helpers
{
    public class Card
    {
        public string Rank { get; }
        public char Suit { get; }
        public string ID => $"{Rank}{Suit}";

        public Card(string rank, char suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public int Value
        {
            get
            {
                return Rank switch
                {
                    "A" => 11,
                    "K" or "Q" or "J" => 10,
                    _ => int.TryParse(Rank, out int val) ? val : 0
                };
            }
        }
    }
}
