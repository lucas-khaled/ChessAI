using System;
using System.Collections.Generic;
using UnityEngine.Profiling;

public class Pawn : SlidingPieces
{
    public Pawn(Board board) : base(board)
    {
    }

    private bool IsOnInitialRow()
    {
        return (Row == 1 && IsWhite)
            || (Row == 6 && !IsWhite);
    }

    private bool CanMoveToDiagonal(List<TileCoordinates> diagonal) 
    {
        if (diagonal.Count <= 0)
            return false;

        var diagonalTile = Board.tiles[diagonal[0].row][diagonal[0].column];
        return IsEnemyPiece(diagonalTile.OccupiedBy)
            || Board.rules.HasEnPassant && diagonalTile.TilePosition.Equals(Board.rules.enPassantTileCoordinates);
    }

    protected override void GenerateBitBoardMethod()
    {
        Profiler.BeginSample("Move Generation > Generate Bitboard -> Pawn");

        int range = IsOnInitialRow() ? 2 : 1;

        var verticals = actualTile.GetVerticalsByColor(pieceColor);
        var checkingBlockVerticals = GetBitboardUntilBlockedSquare(verticals.frontVerticals.GetRange(0, range), false);

        MovingSquares.Add(checkingBlockVerticals);

        Diagonals diagonals = actualTile.GetDiagonalsByColor(pieceColor);

        if (diagonals.topLeftDiagonals.Count > 0)
        {
            var topLeftCoord = diagonals.topLeftDiagonals[0];
            var bitboard = Board.GetTiles()[topLeftCoord.row][topLeftCoord.column].Bitboard;
            if (CanMoveToDiagonal(diagonals.topLeftDiagonals))
                MovingSquares.Add(bitboard);

            AttackingSquares.Add(bitboard);
        }

        if (diagonals.topRightDiagonals.Count > 0)
        {
            var topRightCoord = diagonals.topRightDiagonals[0];
            var bitboard = Board.GetTiles()[topRightCoord.row][topRightCoord.column].Bitboard;

            if (CanMoveToDiagonal(diagonals.topRightDiagonals))
                MovingSquares.Add(bitboard);

            AttackingSquares.Add(bitboard);
        }
        Profiler.EndSample();
    }
}
