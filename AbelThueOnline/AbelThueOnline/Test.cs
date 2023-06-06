using Algorithms;

namespace AbelThueOnline;

public class Test
{
    private int charsCountMin = 3;
    private int charsCountMax = 15;
    private int wordLenMin = 3;
    private int wordLenMax = 15;
    private int games = 10;

    public void Main()
    {
        for (int i = charsCountMin; i < charsCountMax; i++)
        {
             for (int j = wordLenMin; j < wordLenMax; j++)
            {
                var score = RunSingleSetup(i, j, games);
                Console.WriteLine($"n={i}, m={j}: {score}:{games-score}");
            }
        }
    }

    private int RunSingleSetup(int charsCount, int wordLen, int games)
    {
        var p1win = 0;
        for (int i = 0; i < games; i++)
        {
            var game = new Game(charsCount, wordLen);
            IAlgorithm player1 = new RandomAlgorithm();
            IAlgorithm player2 = new MCTS(
                MCTS.Strategy.UCBTuned,
                1000,
                TimeSpan.FromSeconds(5));
            var swap = i % 2 == 0;
            if (swap)
            {
                (player1, player2) = (player2, player1);
            }
            var gameInfo = new Program.GameInfo(game, player1, player2);
            game = gameInfo.Game;
            while (true)
            {
                var action = gameInfo.SelectMove();
                game.PlayerMove(action);

                var availableActions = game.GetAvailableActions();
                if (!availableActions.Any())
                {
                    if ((game.GameState == GameState.Player2Move && !swap) ||
                        (game.GameState == GameState.Player1Move && swap))
                        p1win++;
                    break;
                }
            }

        }

        return p1win;
    }

}