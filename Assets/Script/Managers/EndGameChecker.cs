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
        return GameManager.environment.turnManager.halfMoves >= 50;
    }

    private bool IsStaleMateDraw()
    {
        return GameManager.environment.moveChecker.HasAnyMove() is false;
    }
}
