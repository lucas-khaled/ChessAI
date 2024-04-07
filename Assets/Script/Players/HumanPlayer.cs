using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HumanPlayer : IPlayer
{
    private Tile selectedTile;
    private Move[] actualPossibleMoves;
    private bool canSelect = false;
    private Action<Move> onMove;
    private PieceColor actualColor;
    private GameManager manager;

    public void Init(PieceColor pieceColor)
    {
        actualColor = pieceColor;
        VisualTile.onTileSelected += OnSelectedTile;
    }

    public HumanPlayer(GameManager manager) 
    {
        Debug.Log("Setting human player");
        this.manager = manager;
    }

    ~HumanPlayer() 
    {
        VisualTile.onTileSelected -= OnSelectedTile;
    }

    public void StartTurn(Action<Move> moveCallback)
    {
        canSelect = true;
        onMove = moveCallback;
    }

    private void OnSelectedTile(Tile tile) 
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

    private void SelectTileIfPossible(Tile tile) 
    {
        if (tile.IsOccupied is false) return;

        if (tile.OccupiedBy.pieceColor != actualColor) return;

        selectedTile = tile;

        GetMoves();
    }

    private void GetMoves() 
    {
        actualPossibleMoves = manager.environment.moveMaker.GetMoves(selectedTile.OccupiedBy);

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
        onMove?.Invoke(move);
        canSelect = false;
        DeselectTile();
    }

    public void EndGame()
    {
        canSelect = false;
    }
}
