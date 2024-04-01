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
        if (IsInsuficcientMaterialDraw()) 
        {
            drawType = DrawType.Deadposition;
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

        return count >= 3;
    }

    private bool IsInsuficcientMaterialDraw() 
    {
        var pieces = GameManager.environment.board.pieces;

        if (pieces.Count == 2) return true;

        var notKingPieces = pieces.Where(x => x is not King);

        if (notKingPieces.Any(x => x is Pawn || x is Queen || x is Rook)) return false;

        var notKingWhitePieces = notKingPieces.Where(x => x.pieceColor == PieceColor.White);
        var notKingBlackPieces = notKingPieces.Where(x => x.pieceColor == PieceColor.Black);

        return notKingWhitePieces.Count() < 2 && notKingBlackPieces.Count() < 2;
    }
}
