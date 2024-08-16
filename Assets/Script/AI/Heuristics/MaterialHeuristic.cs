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
        var whitePieces = environment.board.whitePieces;
        var blackPieces = environment.board.blackPieces;
        var heuristic = weight * (QueenMaterialCount(whitePieces, blackPieces) + RookMaterialCount(whitePieces, blackPieces) + BishopMaterialCount(whitePieces, blackPieces) + KnightMaterialCount(whitePieces, blackPieces) + PawnMaterialCount(whitePieces, blackPieces));

        return heuristic;
    }

    private float QueenMaterialCount(List<Piece> whitePieces, List<Piece> blackPieces)
    {
        var score = GetMaterialCount(whitePieces.Where(x => x is Queen).ToList(), blackPieces.Where(x => x is Pawn).ToList(), 10);
        return score;
    }

    private float RookMaterialCount(List<Piece> whitePieces, List<Piece> blackPieces)
    {
        var score = GetMaterialCount(whitePieces.Where(x => x is Rook).ToList(), blackPieces.Where(x => x is Pawn).ToList(), 5);
        return score;
    }

    private float BishopMaterialCount(List<Piece> whitePieces, List<Piece> blackPieces)
    {
        var score = GetMaterialCount(whitePieces.Where(x => x is Bishop).ToList(), blackPieces.Where(x => x is Pawn).ToList(), 3);
        return score;
    }

    private float KnightMaterialCount(List<Piece> whitePieces, List<Piece> blackPieces)
    {
        var score = GetMaterialCount(whitePieces.Where(x => x is Knight).ToList(), blackPieces.Where(x => x is Pawn).ToList(), 3);
        return score;
    }
    private float PawnMaterialCount(List<Piece> whitePieces, List<Piece> blackPieces)
    {
        var score = GetMaterialCount(whitePieces.Where(x => x is Pawn).ToList(), blackPieces.Where(x => x is Pawn).ToList(), 1);
        return score;
    }

    private float GetMaterialCount(List<Piece> whitePieces, List<Piece> blackPieces, float weight)
    {
        int wQnt = whitePieces.Count;
        int bQnt = blackPieces.Count;

        return weight * (wQnt - bQnt);
    }
}
