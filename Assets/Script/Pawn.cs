using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : BlockableMovesPiece
{
    public override Move[] GetMoves()
    {
        List<Move> possibleMoves = new List<Move>();

        possibleMoves.AddRange(GetMoves());
        possibleMoves.AddRange(GetCaptures());
        
        return possibleMoves.ToArray();
    }

    private Move[] GetMoves() 
    {
        int range = (IsOnInitialRow()) ? 2 : 1;

        var verticals = GameManager.Board.GetVerticalsFrom(actualTile.TilePosition, pieceColor, range);
        var checkingBlockVerticals = CheckForBlockingSquares(verticals.frontVerticals, false);
        
        return CreateMovesFromSegment(checkingBlockVerticals);
    }

    private bool IsOnInitialRow()
    {
        return (Row == 1 && IsWhite)
            || (Row == 6 && !IsWhite);
    }

    private Move[] GetCaptures() 
    {
        List<Move> moves = new();

        var diagonals = GameManager.Board.GetDiagonalsFrom(actualTile.TilePosition, pieceColor, 1);

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
