using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialHeuristic : Heuristic
{
    public MaterialHeuristic(GameManager manager, float weight = 1) : base(manager, weight)
    {
    }

    public override float GetHeuristic(Board board)
    {
        var heuristic = weight * (GetMaterialCount(board.piecesHolder.whiteQueens, board.piecesHolder.blackQueens, 10) 
            + GetMaterialCount(board.piecesHolder.whiteRooks, board.piecesHolder.blackRooks, 5)
            + GetMaterialCount(board.piecesHolder.whiteBishops, board.piecesHolder.blackBishops, 3)
            + GetMaterialCount(board.piecesHolder.whiteKnights, board.piecesHolder.blackKnights, 3) 
            + GetMaterialCount(board.piecesHolder.whitePawns, board.piecesHolder.blackPawns, 1));

        return heuristic;
    }

    private float GetMaterialCount<T>(List<T> whitePieces, List<T> blackPieces, float weight) where T : Piece
    {
        int wQnt = whitePieces.Count;
        int bQnt = blackPieces.Count;

        return weight * (wQnt - bQnt);
    }
}
