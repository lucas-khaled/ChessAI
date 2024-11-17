using System;
using UnityEngine;

public class BitboardSelector : MonoBehaviour
{
    [SerializeField] private BitBoardVisualizer boardVisualizer;
    [SerializeField] private bool showAttackingBoard = true;
    [SerializeField] private Color attackingColor = Color.red;
    [SerializeField] private bool showKingDangerBoard = true;
    [SerializeField] private Color kingDangerColor = Color.magenta;

    private void Awake()
    {
        VisualTile.onTileSelected += OnTileSelected;
    }

    private void OnTileSelected(Tile tile)
    {
        Debug.Log("Tile selected: "+tile.visualTile.name);
        if (tile.IsOccupied is false) return;

        tile.OccupiedBy.GenerateBitBoard();

        if(showKingDangerBoard)
            boardVisualizer.SetBitBoard(tile.OccupiedBy.KingDangerSquares, kingDangerColor);

        if (showAttackingBoard)
            boardVisualizer.SetBitBoard(tile.OccupiedBy.AttackingSquares, attackingColor);
        
    }
}
