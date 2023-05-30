using System.Text;
using Algorithms;

namespace AbelThueOnline
{
    public class Player : IAlgorithm
    {
        private Game game;
        private StringBuilder stringBuilder;

        public Player(Game game, StringBuilder stringBuilder)
        {
            this.game = game;
            this.stringBuilder = stringBuilder;
        }

        public int SelectMove(IGame initState, GameState computer)
        {
            var availableActions = initState.GetAvailableActions();
            var action = -1;

            while(!availableActions.Contains(action))
            {
                var errorLine = action != -1
                    ? $"\nInvalid letter, available: {string.Join(", ", availableActions.Select(a => (char)(a + 'a')).ToList())}"
                    : null;

                action = computer switch
                {
                    GameState.Player1Move => ReadPlayer1Move(errorLine),
                    GameState.Player2Move => ReadPlayer2Move(errorLine),
                    _ => throw new InvalidOperationException(),
                };
            }

            return action;
        }

        private int ReadPlayer1Move(string? errorLine)
        {
            var index = 0;
            var key = ConsoleKey.NoName;

            while (key != ConsoleKey.Enter)
            {
                Console.Write(stringBuilder.ToString());
                Console.WriteLine(UpperCaseRepetition(Player1MoveString(game, index)));
                Console.WriteLine(errorLine);

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

        public static string Player1MoveString(Game game, int index)
        {
            return game.Word.Substring(0, index) + "|" + game.Word.Substring(index, game.Word.Length - index);
        }

        private int ReadPlayer2Move(string? errorLine)
        {
            var key = '_';

            while (true)
            {
                Console.Write(stringBuilder.ToString());
                Console.WriteLine(UpperCaseRepetition(Player2MoveString(game, key)));
                Console.WriteLine(errorLine);

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

        public static string Player2MoveString(Game game, char letter)
        {
            return game.Word.Substring(0, game.Index) + letter + game.Word.Substring(game.Index, game.Word.Length - game.Index);
        }

        public override string ToString()
        {
            return "Player";
        }

        private static string UpperCaseRepetition(string word)
        {
            var n = word.Length;
            for (var i = 1; i <= n/2; i++)
            {
                for (var j = 0; j <= n - 2 * i; j++)
                {
                    var a = word[j..(j + i)];
                    var aGrouped = a.GroupBy(x => x).Select(x => new { x.Key, Count = x.Count() }).OrderBy(x => x.Key).ToList();
                    var b = word[(j + i)..(j + 2 * i)];
                    var bGrouped = b.GroupBy(x => x).Select(x => new { x.Key, Count = x.Count() }).OrderBy(x => x.Key).ToList();
                    var repetitionExist = aGrouped.All(a => bGrouped.Where(b => b.Key == a.Key).Select(b => b.Count).FirstOrDefault() == a.Count);
                    if (repetitionExist)
                    {
                        return word[..j] + a.ToUpper() + b.ToUpper() + word[(j + 2 * i)..];
                    }
                }
            }
            return word;
        }
    }
}
