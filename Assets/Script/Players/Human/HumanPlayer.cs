using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HumanPlayer : Player
{
    private Tile selectedTile;
    private Move[] actualPossibleMoves;

    public override void Init(PieceColor pieceColor)
    {
        base.Init(pieceColor);
        VisualTile.onTileSelected += OnSelectedTile;
    }

    public HumanPlayer(GameManager manager) : base(manager)
    {
    }

    ~HumanPlayer() 
    {
        VisualTile.onTileSelected -= OnSelectedTile;
    }

    private void OnSelectedTile(Tile tile) 
    {
        if (canPlay is false) return;

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
            SelectTileIfPossible(tile);
            return;
        }

        if (moves.Any(m => m is PromotionMove))
            SelectPromotion(moves.Cast<PromotionMove>().ToList());
        else
            DoMove(moves[0]);
    }

    private void SelectTileIfPossible(Tile tile) 
    {
        if (tile.IsOccupied is false) return;

        if (tile.OccupiedBy.pieceColor != actualColor) return;

        selectedTile = tile;

        GetMoves();
    }

    private void GetMoves() 
    {
        actualPossibleMoves = manager.GameBoard.currentTurnMoves.Where(move => move.piece.Equals(selectedTile.OccupiedBy)).ToArray();

        SetPossibleTilesMaterial(actualPossibleMoves);
    }

    private void SetPossibleTilesMaterial(Move[] moves) 
    {
        foreach (Move move in moves)
            move.to.visualTile.Paint(Color.yellow);
    }

    private void DeselectTile() 
    {
        ResetPossibleTilesMaterial(actualPossibleMoves);
        selectedTile = null;
        actualPossibleMoves = null;
    }
    private void ResetPossibleTilesMaterial(Move[] moves)
    {
        foreach (Move move in moves)
            move.to.visualTile.Paint();
    }

    private void SelectPromotion(List<PromotionMove> moves) 
    {
        manager.UIManager.ShowPromotionPopup(moves, DoMove);
    }

    private void DoMove(Move move) 
    {
        canPlay = false;
        DeselectTile();
        onMove?.Invoke(move);
    }
}
