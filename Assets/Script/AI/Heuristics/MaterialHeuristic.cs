using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialHeuristic : Heuristic
{
    public MaterialHeuristic(float weight = 1) : base(weight)
    {
    }

    public override float GetHeuristic(Environment environment)
    {
        var pieces = environment.board.pieces;

        return weight * (QueenMaterialCount(pieces) + RookMaterialCount(pieces) + BishopMaterialCount(pieces) + KnightMaterialCount(pieces) + PawnMaterialCount(pieces));
    }

    private float QueenMaterialCount(List<Piece> pieces)
    {
        var score = GetMaterialCount(pieces.Where(x => x is Queen).ToList(), 10);
        return score;
    }

    private float RookMaterialCount(List<Piece> pieces)
    {
        var score = GetMaterialCount(pieces.Where(x => x is Rook).ToList(), 5);
        return score;
    }

    private float BishopMaterialCount(List<Piece> pieces)
    {
        var score = GetMaterialCount(pieces.Where(x => x is Bishop).ToList(), 3);
        return score;
    }

    private float KnightMaterialCount(List<Piece> pieces)
    {
        var score = GetMaterialCount(pieces.Where(x => x is Knight).ToList(), 3);
        return score;
    }
    private float PawnMaterialCount(List<Piece> pieces)
    {
        var score = GetMaterialCount(pieces.Where(x => x is Pawn).ToList(), 1);
        return score;
    }

    private float GetMaterialCount(List<Piece> pieces, float weight)
    {
        int wQnt = pieces.Count(x => x.pieceColor == PieceColor.White);
        int bQnt = pieces.Count(x => x.pieceColor == PieceColor.Black);

        return weight * (wQnt - bQnt);
    }
}
