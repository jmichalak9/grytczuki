namespace Algorithms
{
    public class RandomAlgorithm : IAlgorithm
    {
        private Random random;

        public RandomAlgorithm()
        {
            random = new Random();
        }

        public int SelectMove(IGame initState, GameState computer)
        {
            var actions = initState.GetAvailableActions();

            return actions[random.Next(actions.Count)];
        }
    }
}
