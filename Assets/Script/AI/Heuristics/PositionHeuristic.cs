using System.Collections.Generic;
using System.Linq;

public abstract class PositionHeuristic : Heuristic
{
    protected PositionHeuristic(GameManager manager) : base(manager, 1)
    {
    }

    protected abstract List<Heuristic> heuristics { get; }
    public override float GetHeuristic(Board board)
    {
        return heuristics.Sum(x => x.GetHeuristic(board));
    }
}
