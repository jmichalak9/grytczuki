namespace Algorithms;

public interface IGame
{
    List<int> GetAvailableActions();
    IGame MakeMove(int action);
}
