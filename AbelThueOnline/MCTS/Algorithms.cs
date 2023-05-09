namespace Algorithms
{
    public class Node
    {
        public Node? Parent { get; }
        public IGame State { get; }
        public int? Move { get; }
        public double Value { get; set; }
        public int VisitsCounter { get; set; }
        public bool IsRoot 
        {
            get
            {
                return Parent == null;
            }
        }
        public List<Node> Children { get; set; }
        public List<int> UnexpandedMoves { get; }
        public List<double> Rewards { get; }

        public Node(IGame prevState, Node? parent = null, int? move = null)
        {
            Children = new List<Node>();
            Parent = parent;
            Move = move;
            Rewards = new List<double>();
            State = prevState;
        
            if (move.HasValue)
            {
                State = State.MakeMove(move.Value);
            }

            UnexpandedMoves = State.GetAvailableActions();
        }

        public Node Expand(int move)
        {
            var newNode = new Node(State, this, move);
            UnexpandedMoves.Remove(move);
            Children.Add(newNode);

            return newNode;
        }

        public double CalculateUCB()
        {
            return (Value / VisitsCounter) + (Math.Sqrt(2) * Math.Sqrt(Math.Log(Parent!.VisitsCounter) / VisitsCounter));
        }
        
        public double CalculateUCBMixMax()
        {
            var q = 0.125; // From paper
            var max = Children.MaxBy(x => x.Value) switch
            {
                Node n => n.Value,
                null => 0, // no children found
            };
            var exploitation = q * max + (1 - q) * (Value / VisitsCounter);
            return exploitation + (Math.Sqrt(2) * Math.Sqrt(Math.Log(Parent!.VisitsCounter) / VisitsCounter));
        }

        public double CalculateUCBTuned()
        {
            double sum = Rewards.Sum(x => x * x) / 2.0;

            var vjs = sum + (Value / VisitsCounter) * (Value / VisitsCounter) + Math.Sqrt(Math.Log(Parent!.VisitsCounter) / VisitsCounter);
            return (Value / VisitsCounter) + (Math.Sqrt(2) * Math.Sqrt(Math.Log(Parent!.VisitsCounter) / VisitsCounter * Math.Min(1f / 4f, vjs)));
        }
    }
    public enum Player
    {
        One = 1,
        Two = 2
    }
    public class MCTS
    {
        private Random _rng;
        private Node _root;
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
                    Strategy.UCBTuned => node.Children.MaxBy(x => x.CalculateUCBTuned()) ?? throw new ArgumentException()
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
            UCB = 1,
            UCBMixMax = 2,
            UCBTuned = 3
        }
    }

}
