namespace BlackjackGame.Helpers
{
    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    public class Card
    {
        public string Rank { get; }
        public char SuitChar { get; }
        public string ID => $"{Rank}{SuitChar}";

        public Card(string rank, char suit)
        {
            Rank = rank;
            SuitChar = suit;
        }

        public int Value
        {
            get
            {
                return Rank switch
                {
                    "A" => 14,
                    "K" => 13,
                    "Q" => 12,
                    "J" => 11,
                    _ => int.TryParse(Rank, out int val) ? val : 0
                };
            }
        }

        public Suit Suit =>
            SuitChar switch
            {
                'H' => Suit.Hearts,
                'D' => Suit.Diamonds,
                'C' => Suit.Clubs,
                'S' => Suit.Spades,
                _ => Suit.Spades
            };

        public override string ToString() => $"{Rank} of {Suit}";
    }
}