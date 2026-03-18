using System;

public class Card
{
    public Suit Suit { get; }
    public Rank Rank { get; }

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    private string GetSuitSymbol()
    {
        return Suit switch
        {
            Suit.Hearts => "♥",
            Suit.Diamonds => "♦",
            Suit.Clubs => "♣",
            Suit.Spades => "♠",
            _ => "?"
        };
    }

    private string GetRankSymbol()
    {
        return Rank switch
        {
            Rank.Six => "6",
            Rank.Seven => "7",
            Rank.Eight => "8",
            Rank.Nine => "9",
            Rank.Ten => "10",
            Rank.Jack => "J",
            Rank.Queen => "Q",
            Rank.King => "K",
            Rank.Ace => "A",
            _ => "?"
        };
    }

    public string[] GetAscii()
    {
        string rank = GetRankSymbol();
        string suit = GetSuitSymbol();

        string left = rank.PadRight(2);
        string right = rank.PadLeft(2);

        return new string[]
        {
            "┌─────┐",
            $"│{left}   │",
            $"│  {suit}  │",
            $"│   {right}│",
            "└─────┘"
        };
    }

    public override string ToString()
    {
        return $"{GetRankSymbol()}{GetSuitSymbol()}";
    }
}