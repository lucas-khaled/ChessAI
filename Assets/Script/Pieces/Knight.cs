using System.Collections.Generic;
using UnityEngine.Profiling;

public class Knight : Piece
{
    private Bitboard leftBorder = new Bitboard(0b0000000100000001000000010000000100000001000000010000000100000001);
    private Bitboard rightBorder = new Bitboard(0b1000000010000000100000001000000010000000100000001000000010000000);

    private Bitboard farRightBorder = new Bitboard(0b0100000001000000010000000100000001000000010000000100000001000000);
    private Bitboard farLeftBorder = new Bitboard(0b0000001000000010000000100000001000000010000000100000001000000010);

    public Knight(Board board) : base(board)
    {
    }

    protected override void GenerateBitBoardMethod()
    {
        Profiler.BeginSample("Move Generation > Generate Bitboard -> Knight");
        Bitboard farRightMask = ~(farLeftBorder | leftBorder);
        Bitboard farLeftMask = ~(farRightBorder | rightBorder);
        Bitboard rightMask = ~leftBorder;
        Bitboard leftMask = ~rightBorder;

        var upRight = actualTile.Bitboard << 17 & rightMask;
        var upLeft = actualTile.Bitboard << 15 & leftMask;
        var downRight = actualTile.Bitboard >> 15 & rightMask;
        var downLeft = actualTile.Bitboard >> 17 & leftMask;
        var leftUp = actualTile.Bitboard << 6 & farLeftMask;
        var leftDown = actualTile.Bitboard >> 10 & farLeftMask;
        var rightUp = actualTile.Bitboard << 10 & farRightMask;
        var rightDown = actualTile.Bitboard >> 6 & farRightMask;

        MovingSquares = AttackingSquares = upRight | upLeft | downRight | downLeft | leftUp | leftDown | rightUp | rightDown;

        Profiler.EndSample();
    }
}
