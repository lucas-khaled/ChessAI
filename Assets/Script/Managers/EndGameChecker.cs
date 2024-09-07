using System.Linq;
using UnityEngine;

public struct EndGameInfo
{
    public bool hasEnded;
    public bool isCheckMate;
    public DrawType drawType;
}

public class EndGameChecker
{
    private GameManager manager;

    public EndGameChecker(GameManager manager) 
    {
        SetManager(manager);
    }

    public void SetManager(GameManager manager) 
    {
        this.manager = manager;
    }

    public EndGameInfo CheckEnd(Board board)
    {
        EndGameInfo info = new EndGameInfo();
        if (IsCheckMate(board))
            info.hasEnded = info.isCheckMate = true;
        else if (HasDraw(out info.drawType, board)) 
            info.hasEnded = true;

        return info;
    }

    public bool IsCheckMate(Board board) 
    {
        return manager.MoveChecker.IsCheckMate(board);
    }

    public bool HasDraw(Board board) 
    {
        DrawType _;
        return HasDraw(out _, board);
    }

    public bool HasDraw(out DrawType drawType, Board board) 
    {
        if (IsStaleMateDraw(board))
        {
            drawType = DrawType.Stalemate;
            return true;
        }
        if (Is50MoveDraw(board)) 
        {
            drawType = DrawType.RuleOf50Moves;
            return true;
        }
        if (IsThreefoldDraw(board)) 
        {
            drawType = DrawType.ThreefoldDraw;
            return true;
        }
        if (IsInsuficcientMaterialDraw(board)) 
        {
            drawType = DrawType.Deadposition;
            return true;
        }

        drawType = DrawType.None;
        return false;
    }

    private bool Is50MoveDraw(Board board)
    {
        return board.LastTurn.halfMoves >= 50;
    }

    private bool IsStaleMateDraw(Board board)
    {
        return manager.MoveChecker.HasAnyMove(board) is false;
    }

    private bool IsThreefoldDraw(Board board)
    {
        var moves = board.turns;

        if (moves.Count < 3) return false;

        FEN fen = moves[moves.Count-1].fen;
        int count = moves.Count(x => x.fen.fullPositionsString == fen.fullPositionsString);

        return count >= 3;
    }

    private bool IsInsuficcientMaterialDraw(Board board) 
    {
        if (board.pieces.Count == 2) return true;

        var notKingWhitePieces = board.whitePieces.Where(x => x is not King);
        var notKingBlackPieces = board.blackPieces.Where(x => x is not King);

        if (notKingWhitePieces.Any(x => x is Pawn || x is Queen || x is Rook) || notKingBlackPieces.Any(x => x is Pawn || x is Queen || x is Rook)) return false;

        return notKingWhitePieces.Count() <= 2 && notKingBlackPieces.Count() <= 2;
    }
}
