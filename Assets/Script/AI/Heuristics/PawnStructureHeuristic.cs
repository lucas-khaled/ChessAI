using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PawnStructureHeuristic : Heuristic
{
    private Board board;

    public PawnStructureHeuristic(GameManager manager, float weight = 1) : base(manager, weight)
    {
    }

    public override float GetHeuristic(Board board)
    {
        if(board == null) 
            throw new System.Exception($"[{nameof(PawnStructureHeuristic)}]The environment passed is null");

        this.board = board;
        
        float sumForWhite = GetPawnsHeuristic(board.piecesHolder.whitePawns);
        float sumForBlack = GetPawnsHeuristic(board.piecesHolder.blackPawns);
        float finalHeuristic = weight * -1f * (sumForWhite - sumForBlack);

        return finalHeuristic;
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
        var opositePaws = pawn.pieceColor == PieceColor.White ? board.piecesHolder.blackPawns : board.piecesHolder.whitePawns;

        return opositePaws.Any(x => x.Coordinates.column == pawnColumn);
    }
}
