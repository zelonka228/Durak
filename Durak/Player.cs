
using System;
using System.Collections.Generic;

public class Player
{
    public string Name;
    public List<Card> Hand = new List<Card>();

    public Player(string name)
    {
        Name = name;
    }

    public void AddCard(Card card)
    {
        if (card != null)
            Hand.Add(card);
    }

   
    public void ShowCards(int selectedIndex = -1)
    {
        int startY = Console.WindowHeight - 8;

        for (int i = 0; i < 8; i++)
        {
            Console.SetCursorPosition(0, startY + i);
            Console.Write(new string(' ', Console.WindowWidth));
        }

        Console.SetCursorPosition(0, startY);
        Console.WriteLine($"{Name} карты:\n");

        List<string[]> asciiCards = new List<string[]>();

        foreach (var card in Hand)
            asciiCards.Add(card.GetAscii());

        for (int row = 0; row < 5; row++)
        {
            Console.SetCursorPosition(0, startY + 1 + row);

            for (int i = 0; i < asciiCards.Count; i++)
            {
                if (i == selectedIndex)
                    Console.ForegroundColor = ConsoleColor.Green;

                Console.Write(asciiCards[i][row] + " ");
                Console.ResetColor();
            }
        }

        Console.SetCursorPosition(0, startY + 6);

        for (int i = 0; i < Hand.Count; i++)
        {
            if (i == selectedIndex)
                Console.ForegroundColor = ConsoleColor.Green;

            Console.Write($"   {i}    ");
            Console.ResetColor();
        }

        Console.SetCursorPosition(0, startY + 7);
        Console.Write("← → выбрать | Enter — сыграть | Q — выход     ");
    }



    public Card ChooseCard()
    {
        int index = 0;
        ConsoleKey key;

        while (true)
        {
            ShowCards(index);

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.RightArrow)
                index = (index + 1) % Hand.Count;

            if (key == ConsoleKey.LeftArrow)
                index = (index - 1 + Hand.Count) % Hand.Count;

            if (key == ConsoleKey.Enter)
            {
                var card = Hand[index];
                Hand.RemoveAt(index);
                return card;
            }

            if (key == ConsoleKey.Q)
                Environment.Exit(0);
        }
    }

    
    public Card ChooseCardOrTake(out bool take)
    {
        int index = 0;
        ConsoleKey key;
        take = false;

        while (true)
        {
            ShowCards(index);

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.RightArrow)
                index = (index + 1) % Hand.Count;

            if (key == ConsoleKey.LeftArrow)
                index = (index - 1 + Hand.Count) % Hand.Count;

            if (key == ConsoleKey.Enter)
            {
                var card = Hand[index];
                Hand.RemoveAt(index);
                return card;
            }

          
            if (key == ConsoleKey.E)
            {
                take = true;
                return null;
            }
        }
    }


}
