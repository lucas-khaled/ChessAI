using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePositionHeuristic : PositionHeuristic
{
    public SimplePositionHeuristic(GameManager manager) : base(manager)
    {
    }

    protected override List<Heuristic> heuristics => new List<Heuristic>()
    {
        new MaterialHeuristic(manager),
        new CenterControlHeuristic(manager, 0.5f),
        new PawnStructureHeuristic(manager, 0.3f),
        //new MobilityHeuristic(manager, 0.1f)
    };
}

