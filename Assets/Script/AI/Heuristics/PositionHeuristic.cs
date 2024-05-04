using System.Collections.Generic;
using System.Linq;

public abstract class PositionHeuristic : Heuristic
{
    protected PositionHeuristic() : base(1)
    {
    }

    protected abstract List<Heuristic> heuristics { get; }
    public override float GetHeuristic(Environment environment)
    {
        return heuristics.Sum(x => x.GetHeuristic(environment));
    }
}
