using System;
using UnityEngine;

public class BitboardSelector : MonoBehaviour
{
    enum VisualizationType 
    {
        KingDanger,
        Attacking
    }

    [SerializeField] private BitBoardVisualizer boardVisualizer;
    [SerializeField] private VisualizationType visualizationType = VisualizationType.Attacking;
    [SerializeField] private Color attackingColor = Color.red;
    [SerializeField] private Color kingDangerColor = Color.magenta;

    private void Awake()
    {
        VisualTile.onTileSelected += OnTileSelected;
    }

    private void OnTileSelected(Tile tile)
    {
        Debug.Log("Tile selected: " + tile.visualTile.name);
        if (tile.IsOccupied is false) return;

        tile.OccupiedBy.GenerateBitBoard();

        switch (visualizationType) 
        {
            case VisualizationType.KingDanger:
                boardVisualizer.SetBitBoard(tile.OccupiedBy.KingDangerSquares, kingDangerColor);
                break;
            case VisualizationType.Attacking:
                boardVisualizer.SetBitBoard(tile.OccupiedBy.AttackingSquares, attackingColor);
                break;
        }
    }
}
