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
    public King kingPrefab;
    public Knight knightPrefab;
    public Queen queenPrefab;
    public Bishop bishopPrefab;
    public Rook rookPrefab;
    public Pawn pawnPrefab;

}
