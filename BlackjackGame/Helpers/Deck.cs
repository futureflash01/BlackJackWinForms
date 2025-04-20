using System;
using System.Collections.Generic;

namespace BlackjackGame.Helpers
{
    public class Deck
    {
        private readonly List<Card> cards;
        private readonly Random random = new Random();

        private static readonly string[] Ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        private static readonly char[] Suits = { 'H', 'D', 'C', 'S' };

        public Deck()
        {
            cards = new List<Card>();

            foreach (var suit in Suits)
            {
                foreach (var rank in Ranks)
                {
                    cards.Add(new Card(rank, suit));
                }
            }

            Shuffle();
        }

        public void Shuffle()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                int j = random.Next(cards.Count);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
        }

        public Card DrawCard()
        {
            if (cards.Count == 0)
            {
                return null;
            }

            var card = cards[0];
            cards.RemoveAt(0);

            return card;
        }
    }
}
