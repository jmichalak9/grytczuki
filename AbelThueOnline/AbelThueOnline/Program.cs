﻿using System.Text;
using Algorithms;

namespace AbelThueOnline;

public class Program
{
    public static bool saveGameToTxt = true;

    public static void Main()
    {
        var t = new Test();
        t.Main();
    }

    public static void CMD()
    {
        var isNewGame = true;

        while (isNewGame)
        {
            var fileName = DateTime.Now.ToString("HH-mm-ss-fffffff") + ".txt";
            var sb = new StringBuilder();

            var gameInfo = CreateGame(sb);
            var game = gameInfo.Game;

            if (saveGameToTxt)
            {
                sb.AppendLine($"CharsCount: {gameInfo.Game.CharsCount}");
                sb.AppendLine($"WordLength: {gameInfo.Game.WordLength}");
                sb.AppendLine($"Player1: {gameInfo.Player1.ToString()}");
                sb.AppendLine($"Player2: {gameInfo.Player2.ToString()}\n");
            }

            while (true)
            {
                var action = gameInfo.SelectMove();

                if (saveGameToTxt)
                {
                    sb.AppendLine(
                        game.GameState == GameState.Player1Move
                            ? Player.Player1MoveString(game, action)
                            : Player.Player2MoveString(game, (char)('a' + action)));

                    Console.Clear();
                    Console.Write(sb.ToString());
                }

                game.PlayerMove(action);

                var availableActions = game.GetAvailableActions();
                if (!availableActions.Any())
                {
                    if (saveGameToTxt)
                    {
                        sb.AppendLine(
                            game.GameState == GameState.Player1Move
                                ? "\nPlayer2 won"
                                : "\nPlayer1 won");

                        Console.Clear();
                        Console.Write(sb.ToString());
                    }

                    Console.WriteLine("Press enter to start again or escape to exit");

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

    private static GameInfo CreateGame(StringBuilder sb)
    {
        Console.Clear();

        var game = new Game(ReadCharsCount(), ReadWordLength());
        var player1 = CreateAlgorithm(ReadPlayerType("first"), game, sb);
        var player2 = CreateAlgorithm(ReadPlayerType("second"), game, sb);

        Console.Clear();

        return new(game, player1, player2);
    }

    public class GameInfo
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

    public static IAlgorithm CreateAlgorithm(PlayerType playerType, Game game, StringBuilder sb)
    {
        var strategy = -1;
        var iterationCount = 0;
        var timeout = -1;
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

            while (timeout < 0)
            {
                Console.WriteLine($"Select timeout in seconds (type 0 to disable timeout)");
                int.TryParse(Console.ReadLine(), out timeout);
            }
        }

        return playerType switch
        {
            PlayerType.User => new Player(game, sb),
            PlayerType.Mcts => new MCTS(
                (MCTS.Strategy)strategy,
                iterationCount,
                timeout == 0 ? null : TimeSpan.FromSeconds(timeout)),
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
