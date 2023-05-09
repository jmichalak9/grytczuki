using Algorithms;

namespace AbelThueOnline;

public class Program
{
    public static void Main()
    {
        var isNewGame = true;
        while (isNewGame)
        {
            var gameInfo = CreateGame();
            var game = gameInfo.Game;
            var availableActions = game.GetAvailableActions();
            while (true)
            {
                var action = -1;

                while(!availableActions.Contains(action))
                {
                    if (action != -1)
                    {
                        var actions = string.Join(", ", availableActions.Select(a => (char)(a + 'a')).ToList());
                        Console.WriteLine($@"Invalid letter, available: {actions}");
                    }

                    action = game.GameState switch
                    {
                        GameState.Player1Move => gameInfo.Player1Move(),
                        GameState.Player2Move => gameInfo.Player2Move(),
                        _ => throw new InvalidOperationException(),
                    };
                }


                game.PlayerMove(action);
                availableActions = game.GetAvailableActions();

                if (!availableActions.Any())
                {
                    if (game.GameState == GameState.Player1Move)
                    {
                        Console.WriteLine("Player2 won! Press enter to start again or escape to exit");
                    }
                    else
                    {
                        Console.WriteLine("Player1 won! Press enter to start again or escape to exit");
                    }

                    var key = ConsoleKey.NoName;
                    while (key != ConsoleKey.Enter && key != ConsoleKey.Escape)
                    {
                        key = Console.ReadKey().Key;

                        if (key == ConsoleKey.Escape)
                        {
                            isNewGame = false;
                        }
                    }

                    break;
                }
            }
        }
    }

    private static GameInfo CreateGame()
    {
        var player1 = ReadPlayerType("first");
        var player2 = ReadPlayerType("second");
        var charsCount = ReadCharsCount();
        var wordLength = ReadWordLength();
        return new()
        {
            Game = new(charsCount, wordLength),
            Player1 = player1,
            Player2 = player2,
        };
    }

    private class GameInfo
    {
        public Game Game { get; set; } = null!;
        public PlayerType Player1 { get; set; }
        private MCTS player1MCTS = new MCTS(MCTS.Strategy.UCBTuned, 10);
        public PlayerType Player2 { get; set; }
        private MCTS player2MCTS = new MCTS(MCTS.Strategy.UCBTuned, 10);

        public int Player1Move()
        {
            return Player1 switch 
            {
                PlayerType.User => ReadPlayer1Move(Game),
                PlayerType.Computer => player1MCTS.SelectMove(Game, GameState.Player1Move),
                _ => throw new InvalidOperationException(),
            };
        }

        public int Player2Move()
        {
            return Player2 switch 
            {
                PlayerType.User => ReadPlayer2Move(Game),
                PlayerType.Computer => player2MCTS.SelectMove(Game, GameState.Player2Move),
                _ => throw new InvalidOperationException(),
            };
        }
    }

    private static PlayerType ReadPlayerType(string playerName)
    {
        var player = -1;

        while (player != 0 && player != 1)
        {
            Console.WriteLine($"Select {playerName} player type (0 - user, 1 - computer)");
            int.TryParse(Console.ReadLine(), out player);
        }

        return player switch
        {
            0 => PlayerType.User,
            1 => PlayerType.Computer,
            _ => throw new InvalidOperationException(),
        };
    }

    private static int ReadCharsCount()
    {
        var charsCount = -1;

        while (charsCount <= 0 || charsCount > 24)
        {
            Console.WriteLine($"Select chars count (1-24)");
            int.TryParse(Console.ReadLine(), out charsCount);
        }

        return charsCount;
    }

    private static int ReadWordLength()
    {
        var wordLength = -1;

        while (wordLength <= 0 || wordLength > 100)
        {
            Console.WriteLine($"Select word length (1-100)");
            int.TryParse(Console.ReadLine(), out wordLength);
        }

        return wordLength;
    }

    private static int ReadPlayer1Move(Game game)
    {
        var index = 0;
        var key = ConsoleKey.NoName;

        while (key != ConsoleKey.Enter)
        {
            PrintPlayer1Move(game, index);

            key = Console.ReadKey().Key;
            Console.Clear();

            if (key == ConsoleKey.LeftArrow)
            {
                index = --index  < 0 ? 0 : index;
            }

            if (key == ConsoleKey.RightArrow)
            {
                index = ++index > game.Word.Length ? game.Word.Length : index;
            }
        }

        return index;
    }

    private static void PrintPlayer1Move(Game game, int index)
    {
        Console.WriteLine(game.Word.Substring(0, index) + "|" + game.Word.Substring(index, game.Word.Length - index));
    }

    private static int ReadPlayer2Move(Game game)
    {
        var key = '_';

        while (true)
        {
            PrintPlayer2Move(game, key);

            var readKey = Console.ReadKey();
            Console.Clear();

            if (readKey.Key == ConsoleKey.Enter && key != '_')
            {
                return key - 'a';
            }

            if (readKey.KeyChar - 'a' >= 0 && readKey.KeyChar - 'a' < game.CharsCount)
            {
                key = readKey.KeyChar;
            }
        }
    }

    private static void PrintPlayer2Move(Game game, char letter)
    {
        Console.WriteLine(game.Word.Substring(0, game.Index) + letter + game.Word.Substring(game.Index, game.Word.Length - game.Index));
    }
}
