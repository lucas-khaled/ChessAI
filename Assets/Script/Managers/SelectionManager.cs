using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SelectionManager
{
    private static Tile selectedTile;
    private static Move[] actualPossibleMoves;
    private static bool canSelect = true;

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
        if (canSelect is false) return;

        if(selectedTile == null) 
        {
            SelectTileIfPossible(tile);
            return;
        }

        if (selectedTile == tile) return;

        List<Move> moves = actualPossibleMoves.Where(m => m.to == tile).ToList();
        if (moves is null || moves.Count <= 0) 
        {
            DeselectTile();
            return;
        }

        if (moves.Any(m => m is PromotionMove))
            SelectPromotion(moves.Cast<PromotionMove>().ToList());
        else
            DoMove(moves[0]);
    }

    private static void SelectTileIfPossible(Tile tile) 
    {
        if (tile.IsOccupied is false) return;

        if (tile.OccupiedBy.pieceColor != GameManager.TurnManager.ActualTurn) return;

        selectedTile = tile;

        GetMoves();
    }

    private static void GetMoves() 
    {
        actualPossibleMoves = GameManager.MoveMaker.GetMoves(selectedTile.OccupiedBy);

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

    private static void SelectPromotion(List<PromotionMove> moves) 
    {
        GameManager.UIManager.ShowPromotionPopup(moves, DoMove);
    }

    private static void DoMove(Move move) 
    {
        GameManager.TurnManager.DoMove(move);
        DeselectTile();
    }

    public static void LockSelection() 
    {
        canSelect = false;
    }

    public static void UnlockSelection() 
    {
        canSelect = true;
    }
}
