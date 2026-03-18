using System;
using System.Collections.Generic;

public class Table
{
    public List<(Card attack, Card defense)> Pairs = new();
    public List<Card> DiscardPile = new(); // отбой

    public void AddAttack(Card card)
    {
        Pairs.Add((card, null));
    }

    public void AddDefense(Card card)
    {
        for (int i = 0; i < Pairs.Count; i++)
        {
            if (Pairs[i].defense == null)
            {
                Pairs[i] = (Pairs[i].attack, card);
                return;
            }
        }
    }

    public List<Card> GetAllCards()
    {
        List<Card> cards = new();
        foreach (var p in Pairs)
        {
            cards.Add(p.attack);
            if (p.defense != null)
                cards.Add(p.defense);
        }
        return cards;
    }

    public void ClearToDiscard()
    {
        foreach (var c in GetAllCards())
            DiscardPile.Add(c);

        Pairs.Clear();
    }

    public void Show()
    {
        Console.WriteLine("\n=== СТОЛ ===\n");

        foreach (var pair in Pairs)
        {
            var a = pair.attack.GetAscii();
            var d = pair.defense?.GetAscii();

            for (int i = 0; i < 5; i++)
            {
                Console.Write(a[i] + "   ");
                Console.WriteLine(d != null ? d[i] : "     ");
            }

            Console.WriteLine();
        }
    }
}