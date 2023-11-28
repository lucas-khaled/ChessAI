using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/Pieces Configuration", fileName = "PiecesConfig")]
public class PiecesConfig : ScriptableObject
{
    [Header("Materials")]
    public Material lightMaterial;
    public Material darkMaterial;

    [Header("Prefabs")]
    public VisualPiece kingPrefab;
    public VisualPiece knightPrefab;
    public VisualPiece queenPrefab;
    public VisualPiece bishopPrefab;
    public VisualPiece rookPrefab;
    public VisualPiece pawnPrefab;

    public VisualPiece GetPrefabFromPiece(Piece piece)
    {
        if (piece is Queen)
            return queenPrefab;

        if (piece is King)
            return kingPrefab;

        if (piece is Rook)
            return rookPrefab;

        if (piece is Bishop)
            return bishopPrefab;

        if (piece is Knight)
            return knightPrefab;

        if (piece is Pawn)
            return pawnPrefab;

        return null;
    }
}
