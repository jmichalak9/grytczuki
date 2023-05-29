using Algorithms;

namespace AbelThueOnline
{
    public class Player : IAlgorithm
    {
        private Game game;

        public Player(Game game)
        {
            this.game = game;
        }

        public int SelectMove(IGame initState, GameState computer)
        {
            var availableActions = initState.GetAvailableActions();
            var action = -1;

            while(!availableActions.Contains(action))
            {
                if (action != -1)
                {
                    var actions = string.Join(", ", availableActions.Select(a => (char)(a + 'a')).ToList());
                    Console.WriteLine($@"Invalid letter, available: {actions}");
                }

                action = computer switch
                {
                    GameState.Player1Move => ReadPlayer1Move(),
                    GameState.Player2Move => ReadPlayer2Move(),
                    _ => throw new InvalidOperationException(),
                };
            }

            return action;
        }

        private int ReadPlayer1Move()
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

        private int ReadPlayer2Move()
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
}
