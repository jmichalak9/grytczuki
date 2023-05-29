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
}
