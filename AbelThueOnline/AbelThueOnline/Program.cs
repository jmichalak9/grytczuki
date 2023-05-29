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

            while (true)
            {
                var action = gameInfo.SelectMove();

                game.PlayerMove(action);
                var availableActions = game.GetAvailableActions();

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
        var game = new Game(ReadCharsCount(), ReadWordLength());
        var player1 = CreateAlgorithm(ReadPlayerType("first"), game);
        var player2 = CreateAlgorithm(ReadPlayerType("second"), game);

        return new(game, player1, player2);
    }

    private class GameInfo
    {
        public GameInfo(Game game, IAlgorithm player1, IAlgorithm player2)
        {
            Game = game;
            Player1 = player1;
            Player2 = player2;
        }

        public Game Game { get; private init; }
        public IAlgorithm Player1 { get; private init; }
        public IAlgorithm Player2 { get; private init; }

        public int SelectMove()
        {
            return Game.GameState switch
            {
                GameState.Player1Move => Player1.SelectMove(Game, GameState.Player1Move),
                GameState.Player2Move => Player2.SelectMove(Game, GameState.Player2Move),
                _ => throw new InvalidOperationException(),
            };
        }
    }

    private static PlayerType ReadPlayerType(string playerName)
    {
        var player = -1;

        while (player != (int)PlayerType.User
            && player != (int)PlayerType.Mcts
            && player != (int)PlayerType.Random)
        {
            Console.WriteLine($"Select {playerName} player type ({(int)PlayerType.User} - user, {(int)PlayerType.Mcts} - mcts, {(int)PlayerType.Random} - random)");
            int.TryParse(Console.ReadLine(), out player);
        }

        return (PlayerType)player;
    }

    private static IAlgorithm CreateAlgorithm(PlayerType playerType, Game game)
    {
        var strategy = -1;
        var iterationCount = 0;
        if (playerType == PlayerType.Mcts)
        {
            while (strategy != (int)(MCTS.Strategy.UCB)
                && strategy != (int)(MCTS.Strategy.UCBMixMax)
                && strategy != (int)(MCTS.Strategy.UCBTuned))
            {
                Console.WriteLine($"Select MCTS strategy type ({(int)(MCTS.Strategy.UCB)} - UCB, {(int)(MCTS.Strategy.UCBMixMax)} - UCBMixMax, {(int)(MCTS.Strategy.UCBTuned)} - UCBTuned)");
                int.TryParse(Console.ReadLine(), out strategy);
            }

            while (iterationCount <= 0)
            {
                Console.WriteLine($"Select iteration count (value greater than 0)");
                int.TryParse(Console.ReadLine(), out iterationCount);
            }
        }

        return playerType switch
        {
            PlayerType.User => new Player(game),
            PlayerType.Mcts => new MCTS((MCTS.Strategy)strategy, iterationCount),
            PlayerType.Random => new RandomAlgorithm(),
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
}
