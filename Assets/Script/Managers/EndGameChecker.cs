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
    private IGameManager manager;

    public EndGameChecker(IGameManager manager) 
    {
        SetManager(manager);
    }

    public void SetManager(IGameManager manager) 
    {
        this.manager = manager;
    }

    public EndGameInfo CheckEnd(Board board)
    {
        EndGameInfo info = new EndGameInfo();
        if (board.IsCheckMate)
            info.hasEnded = info.isCheckMate = true;
        else if (HasDraw(out info.drawType, board)) 
            info.hasEnded = true;

        return info;
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
        return board.HasMoves is false && board.IsCheckMate is false;
    }

    private bool IsThreefoldDraw(Board board)
    {
        var moves = board.turns;

        if (moves.Count < 3) return false;

        string hash = moves[moves.Count-1].zobristHash;
        int count = moves.Count(x => x.zobristHash == hash);

        return count >= 3;
    }

    private bool IsInsuficcientMaterialDraw(Board board) 
    {
        if (board.piecesHolder.pieces.Count == 2) return true;

        if (board.piecesHolder.whitePawns.Count > 0 || board.piecesHolder.blackPawns.Count > 0
            || board.piecesHolder.blackQueens.Count > 0 || board.piecesHolder.whiteQueens.Count > 0
            || board.piecesHolder.whiteRooks.Count > 0 || board.piecesHolder.blackRooks.Count > 0) return false;

        return (board.piecesHolder.whiteKnights.Count + board.piecesHolder.whiteBishops.Count <= 2) 
            && (board.piecesHolder.blackKnights.Count + board.piecesHolder.blackBishops.Count <= 2);
    }
}
