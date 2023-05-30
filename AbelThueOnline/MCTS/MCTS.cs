using System.Diagnostics;

namespace Algorithms
{
    public class MCTS : IAlgorithm
    {
        private Random _rng;
        private Node _root = null!;
        private int _maxIterations;
        private GameState _computer;
        private Strategy _strategy;
        private TimeSpan? timeout;

        public MCTS(Strategy strategy, int iterations, TimeSpan? timeout)
        {
            _maxIterations = iterations;
            _rng = new Random();
            _strategy = strategy;
            this.timeout = timeout;
        }

        // MonteCarloTreeSearch
        public int SelectMove(IGame initState, GameState computer)
        {
            _computer = computer;
            _root = new Node(initState);

            var stopWatch = new Stopwatch();
            stopWatch.Restart();
            stopWatch.Start();
            for (int i = 0; i < _maxIterations; i++)
            {
                // Selection and expansion
                var node = SelectionAndExpansion(_root);

                // Simulation
                var result = Simulation(node);

                // Backpropagate
                Backpropagate(node, result);
                if (timeout.HasValue && stopWatch.Elapsed > timeout.Value)
                {
                    break;
                }
            }

            // Select best move
            var bestNode = _root.Children.MaxBy(x => x.Value);

            return bestNode!.Move ?? throw new ArgumentException();
        }

        private Node SelectionAndExpansion(Node node)
        {
            while (!node!.UnexpandedMoves.Any() && node.Children.Any())
            {
                // Select by UCB
                node = _strategy switch
                {
                    Strategy.UCB => node.Children.MaxBy(x => x.CalculateUCB()) ?? throw new ArgumentException(),
                    Strategy.UCBMixMax => node.Children.MaxBy(x => x.CalculateUCBMixMax()) ?? throw new ArgumentException(),
                    Strategy.UCBTuned => node.Children.MaxBy(x => x.CalculateUCBTuned()) ?? throw new ArgumentException(),
                    _ => throw new InvalidOperationException(),
                };
            }

            if (node.UnexpandedMoves.Any())
            {
                var move = node.UnexpandedMoves[_rng.Next() % node.UnexpandedMoves.Count];
                node = node.Expand(move);
            }

            return node;
        }

        // Node should be a leaf in current tree
        private double Simulation(Node node)
        {
            var actions = node.State.GetAvailableActions();
            IGame currentState = node.State;
            while (actions.Count > 0)
            {
                var selectedMove = actions[_rng.Next() % actions.Count];
                currentState = currentState.MakeMove(selectedMove);
                actions = currentState.GetAvailableActions();
            }

            return currentState.GameState == _computer? 0 : 1;
        }

        private void Backpropagate(Node node, double result)
        {
            while(!node!.IsRoot)
            {
                node.Value += result;
                node.VisitsCounter++;
                node.Rewards.Add(result);

                node = node.Parent ?? throw new ArgumentException();
            }

            // Update root
            node.Value += result;
            node.VisitsCounter++;
        }

        public override string ToString()
        {
            var timeoutString = timeout is null ? "no" : $"{timeout.Value.Seconds} s";
            return $"MCTS - {_strategy} strategy, {_maxIterations} iterations, {timeoutString} timeout";
        }

        public enum Strategy
        {
            UCB = 0,
            UCBMixMax = 1,
            UCBTuned = 2,
        }
    }
}
