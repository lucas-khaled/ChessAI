using System;
using System.Collections.Generic;
using UnityEngine.Profiling;

public class King : SlidingPieces
{
    public King(Board board) : base(board)
    {
        
    }

    protected override void GenerateBitBoardMethod()
    {
        Profiler.BeginSample("Move Generation > Generate Bitboard -> King");
        Bitboard attackingTiles = new Bitboard();

        attackingTiles.Add(GetDiagonalBlockedSquares(1));
        attackingTiles.Add(GetVerticalBlockedSquares(1));
        attackingTiles.Add(GetHorizontalBlockedSquares(1));

        AttackingSquares = attackingTiles;
        MovingSquares.Add(AttackingSquares);

        Bitboard castleBitboard = GetCastleBitboard();
        MovingSquares.Add(castleBitboard);
        Profiler.EndSample();
    }

    private Bitboard GetCastleBitboard()
    {
        Bitboard bitboard = new Bitboard();
        int initialRow = (pieceColor == PieceColor.White) ? 0 : 7;

        if(Coordinates.column != 4 || Coordinates.row != initialRow) return bitboard;

        bitboard.Add(Board.GetTiles()[initialRow][Coordinates.column + 2].Bitboard);
        bitboard.Add(Board.GetTiles()[initialRow][Coordinates.column - 2].Bitboard);

        return bitboard;
    }
}
