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

}
