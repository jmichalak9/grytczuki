﻿using Algorithms;

namespace AbelThueOnline
{
    public class Game : IGame
    {
        public string Word { get; private set; }
        public GameState GameState { get; private set; }
        public int CharsCount { get; private set; }
        public int WordLength { get; private set; }
        public int Index { get; private set; }

        private Game() { Word = null!; }

        public Game(int charsCount, int wordLength)
        {
            CharsCount = charsCount;
            WordLength = wordLength;

            this.GameState = Algorithms.GameState.Player1Move;
            Word = string.Empty;
            Index = 0;
        }

        private Game CopyGame()
        {
            return new()
            {
                CharsCount = CharsCount,
                WordLength = WordLength,
                GameState = GameState,
                Word = Word.ToString(),
                Index = Index,
            };
        }

        public List<int> GetAvailableActions()
        {
            return GameState == GameState.Player1Move ? GetPlayer1Actions() : GetPlayer2Actions();
        }

        public IGame MakeMove(int action)
        {
            var game = CopyGame();
            game.PlayerMove(action);

            return game;
        }

        private List<int> GetPlayer1Actions()
        {
            return Word.Length == WordLength ? new() : Enumerable.Range(0, Word.Length + 1).ToList();
        }

        private List<int> GetPlayer2Actions()
        {
            var actions = Enumerable.Range(0, CharsCount).ToList();
            
            return actions.Where(a => CheckIfWordCorrect(GetNewWord(a))).ToList();
        }

        public void PlayerMove(int action)
        {
            if (GameState == GameState.Player1Move)
            {
                Index = action;
                GameState = GameState.Player2Move;
            }
            else
            {
                Word = GetNewWord(action);
                GameState = GameState.Player1Move;
            }
        }

        private string GetNewWord(int letter)
        {
            return Word.Substring(0, Index) + (char)('a' + letter) + Word.Substring(Index, Word.Length - Index);
        }

        private static bool CheckIfWordCorrect(string word)
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
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
