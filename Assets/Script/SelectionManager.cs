using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SelectionManager
{
    private static Tile selectedTile;
    private static Move[] actualPossibleMoves;

    [InitializeOnLoadMethod]
    public static void Init() 
    {
        VisualTile.onTileSelected += OnSelectedTile;
        Application.quitting += Close;
    }

    public static void Close() 
    {
        VisualTile.onTileSelected -= OnSelectedTile;
    }

    private static void OnSelectedTile(Tile tile) 
    {
        if(selectedTile == null) 
        {
            SelectTileIfPossible(tile);
            return;
        }

        if (selectedTile == tile) return;

        Move move = actualPossibleMoves.ToList().Find(m => m.to == tile);
        if (move is null) 
        {
            DeselectTile();
            return;
        }

        DoMove(move);
    }

    private static void SelectTileIfPossible(Tile tile) 
    {
        if (tile.IsOccupied is false) return;

        if (tile.OccupiedBy.pieceColor != TurnManager.ActualTurn) return;

        selectedTile = tile;

        GetMoves();
    }

    private static void GetMoves() 
    {
        actualPossibleMoves = selectedTile.OccupiedBy.GetMoves(GameManager.Board);
        actualPossibleMoves = TurnManager.GetLegalMoves(actualPossibleMoves);

        SetPossibleTilesMaterial(actualPossibleMoves);
    }

    private static void SetPossibleTilesMaterial(Move[] moves) 
    {
        foreach (Move move in moves)
            move.to.visualTile.Paint(Color.yellow);
    }

    private static void DeselectTile() 
    {
        ResetPossibleTilesMaterial(actualPossibleMoves);
        selectedTile = null;
        actualPossibleMoves = null;
    }
    private static void ResetPossibleTilesMaterial(Move[] moves)
    {
        foreach (Move move in moves)
            move.to.visualTile.Paint();
    }

    private static void DoMove(Move move) 
    {
        TurnManager.SetMove(move);
        DeselectTile();
    }
}
