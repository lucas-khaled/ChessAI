using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override Move[] GetPossibleMoves()
    {
        List<Move> possibleMoves = new List<Move>();

        possibleMoves.AddRange(GetMoves());
        possibleMoves.AddRange(GetCaptures());
        
        return possibleMoves.ToArray();
    }

    private bool IsOnInitialRow() 
    {
        return (Row == 1 && IsWhite)
            || (Row == 6 && !IsWhite);
    }

    private Move[] GetMoves() 
    {
        List<Move> moves = new List<Move>();

        if (Row >= GameManager.Board.BoardRowSize - 1) return moves.ToArray();

        int iteration = (IsWhite) ? 1 : -1;
        var firstTile = GameManager.Board.GetTiles()[Row + iteration][Column];

        if (!firstTile.IsOccupied)
            moves.Add(new Move(actualTile, firstTile));

        if (IsOnInitialRow())
        {
            var secondTile = GameManager.Board.GetTiles()[Row + iteration * 2][Column];
            if (!secondTile.IsOccupied)
                moves.Add(new Move(actualTile, secondTile));
        }

        return moves.ToArray();
    }

    private Move[] GetCaptures() 
    {
        List<Move> moves = new();

        var diagonals = GameManager.Board.GetDiagonalsFrom(actualTile.TilePosition, pieceColor);
        var leftDiagonal = (diagonals[0].Count > 0) ? diagonals[0][0] : null;
        var rightDiagonal = (diagonals[1].Count > 0) ? diagonals[1][0] : null;

        if (leftDiagonal != null && IsEnemyPiece(leftDiagonal.OccupiedBy))
            moves.Add(new Move(actualTile, leftDiagonal, rightDiagonal.OccupiedBy));
        
        if(rightDiagonal != null && IsEnemyPiece(rightDiagonal.OccupiedBy))
            moves.Add(new Move(actualTile, rightDiagonal, rightDiagonal.OccupiedBy));

        return moves.ToArray();

    }
}
