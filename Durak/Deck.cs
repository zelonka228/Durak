using System;
using System.Collections.Generic;

public class Deck
{
    public List<Card> Cards = new List<Card>();
    private Random random = new Random();

    public Deck()
    {
        CreateDeck();
        Shuffle();
    }

    private void CreateDeck()
    {
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                Cards.Add(new Card(suit, rank));
            }
        }
    }

    private void Shuffle()
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            int j = random.Next(Cards.Count);
            (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
        }
    }

    public Card Draw()
    {
        if (Cards.Count == 0)
            return null;

        Card card = Cards[0];
        Cards.RemoveAt(0);
        return card;
    }
}