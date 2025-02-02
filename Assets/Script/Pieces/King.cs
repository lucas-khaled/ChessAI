using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class King : SlidingPieces
{
    public King(Board board) : base(board)
    {
        
    }

    protected override void GenerateBitBoardMethod()
    {
        Profiler.BeginSample("Move Generation > Generate Bitboard -> King");

        var verticalUp = actualTile.Bitboard << 8;
        var verticalDown = actualTile.Bitboard >> 8;
        var horizontalLeft = actualTile.Bitboard >> 1;
        var horizontalRight = actualTile.Bitboard << 1;
        var diagonalUpLeft = actualTile.Bitboard << 7;
        var diagonalUpRight = actualTile.Bitboard << 9;
        var diagonalDownLeft = actualTile.Bitboard >> 9;
        var diagonalDownRight = actualTile.Bitboard >> 7;

        AttackingSquares = verticalUp | verticalDown | horizontalLeft | horizontalRight | diagonalUpLeft | diagonalUpRight | diagonalDownLeft | diagonalDownRight;
        MovingSquares.Add(AttackingSquares);

        Bitboard castleBitboard = GetCastleBitboard();
        MovingSquares.Add(castleBitboard);
        Profiler.EndSample();
    }

    private Bitboard GetCastleBitboard()
    {
        Bitboard bitboard = new Bitboard();
        Bitboard initialSquare = (pieceColor == PieceColor.White) ? new Bitboard(4) : new Bitboard(60);

        if(actualTile.Bitboard != initialSquare) return bitboard;

        bitboard.Add(actualTile.Bitboard >> 2);
        bitboard.Add(actualTile.Bitboard << 2);

        return bitboard;
    }
}
