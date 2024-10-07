using System;
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
        int index = 0;
        foreach (var move in moves)
        {
            try
            {
                gameManager.TurnManager.DoMove(move, board);

                if (checkChecker.IsCheck(board, turn) is false)
                    validMoves.Add(move);

                gameManager.TurnManager.UndoLastMove(board);
            }
            catch (Exception e)
            {
                Debug.LogError($"Found error:\n{e}\n\n FEN is {board.FENManager.GetFEN()} and move is {move}");
            }
            index++;
        }

        return validMoves;
    }

    public bool IsCheckMate(Board board) 
    {
        return checkChecker.IsCheck(board, board.ActualTurn) && HasAnyMove(board) is false;
    }

    public bool HasAnyMove(Board board)
    {
        List<Piece> pieces = board.ActualTurn == PieceColor.White ? board.piecesHolder.whitePieces : board.piecesHolder.blackPieces;
        for (int i = 0; i < pieces.Count; i++)
        {
            var piece = pieces[i];
            var legalMoves = HasAnyLegalMove(piece.GetMoves());
            if (legalMoves)
                return true;
        }

        return false;
    }

    public bool HasAnyLegalMove(Move[] moves) 
    {
        Board board = gameManager.TestBoard;
        PieceColor turn = board.ActualTurn;
        foreach (var move in moves)
        {
            bool valid = false;
            gameManager.TurnManager.DoMove(move, board);

            if (checkChecker.IsCheck(board, turn) is false)
                valid = true;

            gameManager.TurnManager.UndoLastMove(board);

            if (valid) return true;
        }

        return false;
    }
}
