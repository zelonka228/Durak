
using System;
using System.Linq;
using System.Collections.Generic;

public class Game
{
    Deck deck;
    Player player;
    Player bot;
    Table table;

    Suit trump;
    Card trumpCard;

    bool godMode;
    bool playerAttacking = true;

    public void Start()
    {
        Console.CursorVisible = false;
        Console.SetWindowSize(120, 40);
        Console.SetBufferSize(120, 40);

        Console.Write("Включить всевидящий режим?(видеть карты бота) (y/n): ");
        godMode = Console.ReadLine().ToLower() == "y";

        deck = new Deck();
        player = new Player("Игрок");
        bot = new Player("Бот");
        table = new Table();

        Deal();
        SetTrump();

        GameLoop();
    }

    void Deal()
    {
        for (int i = 0; i < 6; i++)
        {
            player.AddCard(deck.Draw());
            bot.AddCard(deck.Draw());
        }
    }

    void DrawUp(Player p)
    {
        while (p.Hand.Count < 6 && deck.Cards.Count > 0)
            p.AddCard(deck.Draw());
    }

    void SetTrump()
    {
        trumpCard = deck.Cards.Last();
        trump = trumpCard.Suit;
    }

    void GameLoop()
    {
        while (true)
        {
            if (playerAttacking)
                PlayerTurn();
            else
                BotTurn();

            DrawUp(player);
            DrawUp(bot);

            CheckWinner();
        }
    }

    // ================= ХОД ИГРОКА =================

    void PlayerTurn()
    {
        table.Pairs.Clear();

        ShowState();
        WriteCentered("ТЫ АТАКУЕШЬ");

        var attack = player.ChooseCard();
        table.Pairs.Add((attack, null));

        while (true)
        {
            if (!BotDefend())
            {
                bot.Hand.AddRange(table.GetAllCards());
                table.Pairs.Clear();

                Pause("Бот забрал карты");
                return;
            }

            if (!CanThrow(player, bot))
                break;

            ShowState();
            WriteCentered("Подкинуть? (Y/N)");

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.Y)
                PlayerThrow();
            else
                break;
        }

        table.ClearToDiscard();
        playerAttacking = false;
    }

    // ================= ХОД БОТА =================

    void BotTurn()
    {
        table.Pairs.Clear();

        ShowState();
        Pause("БОТ АТАКУЕТ");

        var attack = bot.Hand.OrderBy(c => c.Rank).First();
        bot.Hand.Remove(attack);
        table.Pairs.Add((attack, null));

        while (true)
        {
            if (!PlayerDefend())
            {
                player.Hand.AddRange(table.GetAllCards());
                table.Pairs.Clear();

                Pause("Ты забрал карты");
                playerAttacking = false;
                return;
            }

            if (!CanThrow(bot, player))
                break;

            BotThrow();
        }

        table.ClearToDiscard();
        playerAttacking = true;
    }

    // ================= ЗАЩИТА =================

    bool BotDefend()
    {
        for (int i = 0; i < table.Pairs.Count; i++)
        {
            var pair = table.Pairs[i];

            if (pair.defense != null)
                continue;

            var defend = bot.Hand
                .Where(c => CanBeat(c, pair.attack))
                .OrderBy(c => c.Suit == trump)
                .ThenBy(c => c.Rank)
                .FirstOrDefault();

            if (defend == null)
                return false;

            bot.Hand.Remove(defend);
            table.Pairs[i] = (pair.attack, defend);

            ShowState();
            Pause($"Бот побил {pair.attack}");
        }

        return true;
    }

    bool PlayerDefend()
    {
        for (int i = 0; i < table.Pairs.Count; i++)
        {
            var pair = table.Pairs[i];

            if (pair.defense != null)
                continue;

            while (true)
            {
                ShowState();
                WriteCentered($"Побей {pair.attack} | Enter — бить | E — взять");

                bool take;
                var defend = player.ChooseCardOrTake(out take);

                if (take)
                    return false;

                if (!CanBeat(defend, pair.attack))
                {
                    Pause("Нельзя побить!");
                    player.Hand.Add(defend);
                    continue;
                }

                table.Pairs[i] = (pair.attack, defend);
                break;
            }
        }

        return true;
    }

    // ================= ПОДКИДЫВАНИЕ =================

    void PlayerThrow()
    {
        var ranks = table.GetAllCards().Select(c => c.Rank).Distinct();

        var card = player.Hand.FirstOrDefault(c => ranks.Contains(c.Rank));

        if (card == null) return;

        player.Hand.Remove(card);
        table.Pairs.Add((card, null));
    }

    void BotThrow()
    {
        var ranks = table.GetAllCards().Select(c => c.Rank).Distinct();

        var card = bot.Hand.FirstOrDefault(c => ranks.Contains(c.Rank));

        if (card == null) return;

        bot.Hand.Remove(card);
        table.Pairs.Add((card, null));

        ShowState();
        Pause($"Бот подкинул {card}");
    }

    bool CanThrow(Player attacker, Player defender)
    {
        var ranks = table.GetAllCards().Select(c => c.Rank).Distinct();

        return attacker.Hand.Any(c => ranks.Contains(c.Rank))
            && table.Pairs.Count < defender.Hand.Count;
    }

    // ================= ЛОГИКА =================

    bool CanBeat(Card defend, Card attack)
    {
        if (defend.Suit == attack.Suit && defend.Rank > attack.Rank)
            return true;

        if (defend.Suit == trump && attack.Suit != trump)
            return true;

        return false;
    }

    void CheckWinner()
    {
        if (player.Hand.Count == 0 && deck.Cards.Count == 0)
        {
            ShowState();
            Pause("ТЫ ПОБЕДИЛ!");
            Environment.Exit(0);
        }

        if (bot.Hand.Count == 0 && deck.Cards.Count == 0)
        {
            ShowState();
            Pause("БОТ ПОБЕДИЛ!");
            Environment.Exit(0);
        }
    }

    // ================= UI =================

    void ShowState()
    {
        Console.Clear();

        WriteCentered("♠♥ DURAK GAME ♦♣\n");

        WriteCentered("БОТ");
        if (godMode)
            DrawAsciiCentered(bot.Hand.Select(c => c.GetAscii()).ToList());
        else
            DrawAsciiCentered(GetHiddenCards(bot.Hand.Count));

        Console.WriteLine("\n");

        DrawDeckWithTrump();

        Console.WriteLine("\n");

        DrawTable();

        Console.WriteLine("\n");

        WriteCentered("ТЫ");
        DrawAsciiCentered(player.Hand.Select(c => c.GetAscii()).ToList());
    }

    void DrawTable()
    {
        WriteCentered("=== СТОЛ ===");

        if (!table.Pairs.Any())
        {
            WriteCentered("Пусто");
            return;
        }

        foreach (var pair in table.Pairs)
        {
            var cards = new List<string[]>();
            cards.Add(pair.attack.GetAscii());

            if (pair.defense != null)
                cards.Add(pair.defense.GetAscii());

            DrawAsciiCentered(cards);
            Console.WriteLine();
        }
    }

    void DrawDeckWithTrump()
    {
        var back = new string[]
        {
            "┌─────┐",
            "│░░░░░│",
            "│░░░░░│",
            "│░░░░░│",
            "└─────┘"
        };

        var trumpAscii = trumpCard.GetAscii();

        int x = Console.WindowWidth / 2 - 10;

        for (int i = 0; i < 5; i++)
        {
            Console.SetCursorPosition(x, Console.CursorTop);
            Console.Write(back[i]);

            Console.SetCursorPosition(x + 4, Console.CursorTop);
            Console.WriteLine(trumpAscii[i]);
        }

        WriteCentered($"Козырь: {trump}");
    }

    List<string[]> GetHiddenCards(int count)
    {
        var back = new string[]
        {
            "┌─────┐",
            "│░░░░░│",
            "│░░░░░│",
            "│░░░░░│",
            "└─────┘"
        };

        return Enumerable.Repeat(back, count).ToList();
    }

    void DrawAsciiCentered(List<string[]> cards)
    {
        int totalWidth = cards.Count * 8;
        int startX = Math.Max(0, (Console.WindowWidth - totalWidth) / 2);

        for (int row = 0; row < 5; row++)
        {
            Console.SetCursorPosition(startX, Console.CursorTop);

            foreach (var card in cards)
                Console.Write(card[row] + " ");

            Console.WriteLine();
        }
    }

    void WriteCentered(string text)
    {
        int left = Math.Max(0, (Console.WindowWidth - text.Length) / 2);
        Console.SetCursorPosition(left, Console.CursorTop);
        Console.WriteLine(text);
    }

    void Pause(string text)
    {
        WriteCentered(text);
        Console.ReadKey();
    }
}
