using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pastra.Models
{
    public class Deck : Card, IEnumerable
    {
        const int NumOfCards = 52;
        public Card[] fullDeck;
        private Card[] remainingDeck;

        public Deck()
        {
            remainingDeck = new Card[NumOfCards];
        }

        public Card[] getDeck { get { return remainingDeck; } }

        public void InstantiateDeck()
        {
            int i = 0;
            foreach (SUIT s in Enum.GetValues(typeof(SUIT)))
            {
                foreach (VALUE v in Enum.GetValues(typeof(VALUE)))
                {
                    remainingDeck[i] = new Card
                    {
                        ID = i,
                        Suit = s,
                        Value = v,
                        Pastra = 0
                    };
                    i++;
                }
            }

            foreach (Card card in remainingDeck)
            {
                getPoints(card);
            }
        }

        private void getPoints(Card card)
        {
            int points = 0;

            switch (card.Value)
            {
                case VALUE.ACE:
                    points = 1;
                    break;
                case VALUE.TWO:
                    if (card.Suit.Equals(Card.SUIT.CLUBS))
                    {
                        points = 2;
                    }
                    break;
                case VALUE.TEN:
                    if (card.Suit.Equals(Card.SUIT.DIAMONDS))
                    {
                        points = 3;
                    }
                    break;
                case VALUE.JACK:
                    points = 1;
                    break;
                default:
                    points = 0;
                    break;
            }

            card.Points = points;
        }

        public void ShuffleDeck()
        {
            Random rng = new Random();

            Card temp;

            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < NumOfCards; j++)
                {
                    int secondCardIndex = rng.Next(13);
                    temp = remainingDeck[j];
                    remainingDeck[j] = remainingDeck[secondCardIndex];
                    remainingDeck[secondCardIndex] = temp;
                }
            }
            fullDeck = remainingDeck;
        }

        private Card Draw()
        {
            Card drawnCard = remainingDeck[0];
            remainingDeck = remainingDeck.Where(card => card != drawnCard).ToArray();
            return drawnCard;
        }

        public List<Card> DrawHand()
        {
            List<Card> hand = new List<Card>();

            for (int i = 0; i < Global.HandSize; i++)
            {
                hand.Add(Draw());
            };

            return hand;
        }

        public IEnumerator GetEnumerator()
        {
            return remainingDeck.GetEnumerator();
        }
    }
}