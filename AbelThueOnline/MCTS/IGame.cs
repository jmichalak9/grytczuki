namespace Algorithms;

public interface IGame
{
    /// <remarks>
    /// If empty list then player lost.
    /// </remarks>
    List<int> GetAvailableActions();
    /// <remarks>
    /// Returns new instance.
    /// </remarks>
    IGame MakeMove(int action);
    /// <remarks>
    /// Needed to check who lost.
    /// </remarks>
    bool IsFirstPlayerMove { get; set; }
}
