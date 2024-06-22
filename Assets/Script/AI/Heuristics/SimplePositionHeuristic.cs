using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePositionHeuristic : PositionHeuristic
{
    protected override List<Heuristic> heuristics => new List<Heuristic>()
    {
        new MaterialHeuristic(),
        new PawnStructureHeuristic(0.3f),
        new MobilityHeuristic(0.1f)
    };
}

