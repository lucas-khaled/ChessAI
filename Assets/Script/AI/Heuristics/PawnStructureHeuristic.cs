using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class PawnStructureHeuristic : Heuristic
{
    private List<Pawn> bPawns = new();
    private List<Pawn> wPawns = new();
    private Environment environment;

    public PawnStructureHeuristic(float weight = 1) : base(weight)
    {
    }

    public override float GetHeuristic(Environment environment)
    {
        if(environment == null) 
            throw new System.Exception($"[{nameof(PawnStructureHeuristic)}]The environment passed is null");

        this.environment = environment;
        GetAllPawns();
        
        float sumForWhite = GetPawnsHeuristic(wPawns);
        float sumForBlack = GetPawnsHeuristic(bPawns);
        float finalHeuristic = weight * -1f * (sumForWhite - sumForBlack);

        return finalHeuristic;
    }

    private void GetAllPawns() 
    {
        foreach (var piece in environment.board.pieces)
        {
            if (piece is not Pawn pawn) continue;

            if (piece.pieceColor == PieceColor.White)
                wPawns.Add(pawn);
            else
                bPawns.Add(pawn);
        }
    }

    private float GetPawnsHeuristic(List<Pawn> pawns)
    {
        float score = 0;
        for(int i = 0; i<pawns.Count; i++) 
        {
            if (HasDoubledPawn(pawns, i))
                score += 0.5f;
            if (HasIsolatedPawn(pawns, i)) 
                score++;
            if (HasBlockedPawns(pawns[i]))
                score++;
        }

        return score;
    }

    private bool HasDoubledPawn(List<Pawn> pawns, int index) 
    {
        var pawn = pawns[index];
        var pawnColumn = pawn.GetTile().TilePosition.column;
       
        return pawns.Any(x => x.Equals(pawn) is false && x.GetTile().TilePosition.column == pawnColumn);
    }

    private bool HasIsolatedPawn(List<Pawn> pawns, int index) 
    {
        var pawn = pawns[index];
        var pawnColumn = pawn.GetTile().TilePosition.column;

        return pawns.Any(x => x.GetTile().TilePosition.column + 1 == pawnColumn || x.GetTile().TilePosition.column - 1 == pawnColumn) is false;
    }

    private bool HasBlockedPawns(Pawn pawn)
    {
        var pawnColumn = pawn.GetTile().TilePosition.column;
        var opositePaws = pawn.pieceColor == PieceColor.White ? bPawns : wPawns;

        return opositePaws.Any(x => x.Coordinates.column == pawnColumn);
    }
}
