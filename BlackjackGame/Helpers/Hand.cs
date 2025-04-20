using System.Collections.Generic;

namespace BlackjackGame.Helpers
{
    public class Hand
    {
        public List<Card> Cards { get; } = new List<Card>();

        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        public int GetValue()
        {
            int value = 0;
            int aceCount = 0;

            foreach (var card in Cards)
            {
                switch (card.Rank)
                {
                    case "J":
                    case "Q":
                    case "K":
                        value += 10;
                        break;
                    case "A":
                        value += 11;
                        aceCount++;
                        break;
                    default:
                        value += int.Parse(card.Rank);
                        break;
                }
            }

            while (value > 21 && aceCount > 0)
            {
                value -= 10;
                aceCount--;
            }

            return value;
        }
    }
}
