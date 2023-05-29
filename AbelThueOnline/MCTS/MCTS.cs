namespace Algorithms
{
    public class MCTS : IAlgorithm
    {
        private Random _rng;
        private Node _root = null!;
        private int _maxIterations;
        private GameState _computer;
        private Strategy _strategy;

        public MCTS(Strategy strategy, int iterations = 1000)
        {
            _maxIterations = iterations;
            _rng = new Random();
            _strategy = strategy;
        }

        // MonteCarloTreeSearch
        public int SelectMove(IGame initState, GameState computer)
        {
            _computer = computer;
            _root = new Node(initState);

            for (int i = 0; i < _maxIterations; i++)
            {
                // Selection and expansion
                var node = SelectionAndExpansion(_root);

                // Simulation
                var result = Simulation(node);

                // Backpropagate
                Backpropagate(node, result);
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
            // var currentState = new Othello(node.State.Board, node.State.Player);

            // var endInfo = currentState.IsEnd();
            var actions = node.State.GetAvailableActions();
            IGame currentState = node.State;
            while (actions.Count > 0)
            {
                var selectedMove = actions[_rng.Next() % actions.Count];
                currentState = currentState.MakeMove(selectedMove);
                actions = currentState.GetAvailableActions();
                // currentState.MakeMove(selectedMove.Row, selectedMove.Col);
                // endInfo = currentState.IsEnd();
            }

            return currentState.GameState == _computer? 0 : 1;
            // var result = endInfo.blackScore.CompareTo(endInfo.whiteScore);

            // if (_computer == Pawns.Black)
            // {
            //     return result < 0 ? 0 : result == 0 ? 0.5 : 1;
            // }
            // else
            // {
            //     return result > 0 ? 0 : result == 0 ? 0.5 : 1;
            // }

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
            return $"MCTS - {_strategy} strategy, {_maxIterations} iterations";
        }

        public enum Strategy
        {
            UCB = 0,
            UCBMixMax = 1,
            UCBTuned = 2,
        }
    }
}
