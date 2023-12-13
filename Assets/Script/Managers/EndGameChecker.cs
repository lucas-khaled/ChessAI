using System;
using UnityEngine;

public class EndGameChecker
{
    public void DoCheck(PieceColor lastTurnColor) 
    {
        DrawType drawType;
        if (GameManager.environment.moveChecker.IsCheckMate())
        {
            GameManager.UIManager.ShowCheckmateMessage(lastTurnColor);
            SelectionManager.LockSelection();
        }
        else if (HasDraw(out drawType)) 
        {
            GameManager.UIManager.ShowDrawMessage(drawType);
            SelectionManager.LockSelection();
        }
    }

    public bool HasDraw(out DrawType drawType) 
    {
        if (IsStaleMateDraw())
        {
            drawType = DrawType.Stalemate;
            return true;
        }
        if (Is50MoveDraw()) 
        {
            drawType = DrawType.RuleOf50Moves;
            return true;
        }

        drawType = DrawType.None;
        return false;
    }

    private bool Is50MoveDraw()
    {
        var lastSignificantMove = GameManager.environment.turnManager.moves.FindLastIndex(m => m.capture != null || m.piece is Pawn) + 1;
        var totalMoves = GameManager.environment.turnManager.moves.Count;

        var diff = totalMoves - lastSignificantMove;
        return diff >= 50;
    }

    private bool IsStaleMateDraw()
    {
        return GameManager.environment.moveChecker.HasAnyMove() is false;
    }
}
