using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : BlockableMovesPiece
{
    public override Move[] GetMoves(Board board)
    {
        List<Move> possibleMoves = new List<Move>();

        possibleMoves.AddRange(GetFowardMoves(board));
        possibleMoves.AddRange(GetCaptures(board));
        
        return possibleMoves.ToArray();
    }

    private Move[] GetFowardMoves(Board board) 
    {
        int range = (IsOnInitialRow()) ? 2 : 1;

        var verticals = GameManager.BoardManager.GetVerticalsFrom(board, actualTile.TilePosition, pieceColor, range);
        var checkingBlockVerticals = CheckForBlockingSquares(verticals.frontVerticals, false);
        
        return CreateMovesFromSegment(checkingBlockVerticals);
    }

    private bool IsOnInitialRow()
    {
        return (Row == 1 && IsWhite)
            || (Row == 6 && !IsWhite);
    }

    private Move[] GetCaptures(Board board) 
    {
        List<Move> moves = new();

        var diagonals = GameManager.BoardManager.GetDiagonalsFrom(board, actualTile.TilePosition, pieceColor, 1);

        if(CanMoveToDiagonal(diagonals.topLeftDiagonals))
            moves.AddRange(CreateMovesFromSegment(diagonals.topLeftDiagonals));

        if (CanMoveToDiagonal(diagonals.topRightDiagonals))
            moves.AddRange(CreateMovesFromSegment(diagonals.topRightDiagonals));

        return moves.ToArray();
    }

    private bool CanMoveToDiagonal(List<Tile> diagonal) 
    {
        return diagonal.Count > 0 && IsEnemyPiece(diagonal[0].OccupiedBy);
    }
}
