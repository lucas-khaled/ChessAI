using System.Collections.Generic;

public class MobilityHeuristic : Heuristic
{
    public MobilityHeuristic(GameManager manager, float weight) : base(manager, weight)
    {
    }

    public override float GetHeuristic(Board board)
    {
        return 0;
    }
}
