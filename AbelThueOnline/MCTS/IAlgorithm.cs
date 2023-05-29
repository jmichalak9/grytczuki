namespace Algorithms;

public interface IAlgorithm
{
    int SelectMove(IGame initState, GameState computer);
    string ToString();
}
