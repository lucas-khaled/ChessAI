using System.Collections.Generic;
using UnityEngine;

public class MoveChecker
{
    private CheckChecker checkChecker = new();
    private GameManager gameManager;

    public MoveChecker(GameManager manager) 
    {
        gameManager = manager;
    }

    public Move[] GetMoves(Piece piece)
    {
        var pieceMoves = piece.GetMoves();
        var moves = GetLegalMoves(pieceMoves);

        return moves;
    }

    public Move[] GetLegalMoves(Move[] moves)
    {
        var returningMoves = FilterCheckMoves(moves);

        return returningMoves.ToArray();
    }

    private List<Move> FilterCheckMoves(Move[] moves)
    {
        Board board = gameManager.TestBoard;
        List<Move> validMoves = new List<Move>();
        PieceColor turn = board.ActualTurn;
        foreach (var move in moves)
        {
            gameManager.TurnManager.DoMove(move, board);

            if (checkChecker.IsCheck(board, turn) is false)
                validMoves.Add(move);

            gameManager.TurnManager.UndoLastMove(board);
        }

        return validMoves;
    }

    public bool IsCheckMate(Board board) 
    {
        return checkChecker.IsCheck(board, board.ActualTurn) && HasAnyMove(board) is false;
    }

    public bool HasAnyMove(Board board)
    {
        return GetAllPossibleMoves(board).Length > 0;
    }

    public Move[] GetAllPossibleMoves(Board board) 
    {
        List<Piece> pieces = board.ActualTurn == PieceColor.White ? board.whitePieces : board.blackPieces;
        List<Move> moves = new();
        for(int i = 0; i < pieces.Count; i++)
        {
            var piece = pieces[i];
            var legalMoves = GetLegalMoves(piece.GetMoves());
            moves.AddRange(legalMoves);
        }

        return moves.ToArray();
    }
}
