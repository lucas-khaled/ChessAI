using System;
using System.Collections.Generic;
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

    MoveGenerator generator;

    private void Awake()
    {
        VisualTile.onTileSelected += OnTileSelected;
    }

    private void Start()
    {
        generator = new MoveGenerator(boardVisualizer.GameBoard);
    }

    private void OnTileSelected(Tile tile)
    {
        Debug.Log("Tile selected: " + tile.visualTile.name);
        if (tile.IsOccupied is false) return;

        var moves = generator.GenerateMoves(tile.OccupiedBy.pieceColor);
        boardVisualizer.SetBitBoard(moves.GetBitboard(), attackingColor);
        /*tile.OccupiedBy.GenerateBitBoard();

        switch (visualizationType) 
        {
            case VisualizationType.KingDanger:
                boardVisualizer.SetBitBoard(tile.OccupiedBy.KingDangerSquares, kingDangerColor);
                break;
            case VisualizationType.Attacking:
                boardVisualizer.SetBitBoard(tile.OccupiedBy.AttackingSquares, attackingColor);
                break;
        }*/


    }
}