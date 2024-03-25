using System;
using System.Linq;
using UnityEngine;

public class EndGameChecker
{
    FENManager FENManager = new FENManager();
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
        if (IsThreefoldDraw()) 
        {
            drawType = DrawType.ThreefoldDraw;
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

    private bool IsThreefoldDraw()
    {
        var moves = GameManager.environment.turnManager.moves;
        FEN fen = moves[moves.Count-1].fen;
        int count = GameManager.environment.turnManager.moves.Count(x => x.fen.fullPositionsString == fen.fullPositionsString);

        Debug.Log("Count: "+count);
        return count >= 3;
    }
}
