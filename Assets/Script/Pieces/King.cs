using UnityEngine;
using UnityEngine.Profiling;

public class King : SlidingPieces
{
    private Bitboard leftBorder = new Bitboard(0b0000000100000001000000010000000100000001000000010000000100000001);
    //new Bitboard(0) | new Bitboard(8) | new Bitboard(16) | new Bitboard(24) | new Bitboard(32) | new Bitboard(40) | new Bitboard(48) | new Bitboard(56);

    private Bitboard rightBorder = new Bitboard(0b1000000010000000100000001000000010000000100000001000000010000000);
        //new Bitboard(7) | new Bitboard(15) | new Bitboard(23) | new Bitboard(31) | new Bitboard(39) | new Bitboard(47) | new Bitboard(55) | new Bitboard(63);
    public King(Board board) : base(board)
    {
        
    }

    protected override void GenerateBitBoardMethod()
    {
        Profiler.BeginSample("Move Generation > Generate Bitboard -> King");

        Bitboard rightMask = ~leftBorder;
        Bitboard leftMask = ~rightBorder;

        var verticalUp = actualTile.Bitboard << 8;
        var verticalDown = actualTile.Bitboard >> 8;
        var horizontalLeft = actualTile.Bitboard >> 1 & leftMask;
        var horizontalRight = actualTile.Bitboard << 1 & rightMask;
        var diagonalUpLeft = actualTile.Bitboard << 7 & leftMask;
        var diagonalUpRight = actualTile.Bitboard << 9 & rightMask;
        var diagonalDownLeft = actualTile.Bitboard >> 9 & leftMask;
        var diagonalDownRight = actualTile.Bitboard >> 7 & rightMask;

        Debug.Log($"{nameof(verticalUp)} => {verticalUp.ToVisualString()}\n" +
            $"{nameof(verticalDown)} => {verticalDown.ToVisualString()}\n" +
            $"{nameof(horizontalLeft)} => {horizontalLeft.ToVisualString()}\n" +
            $"{nameof(horizontalRight)} => {horizontalRight.ToVisualString()}\n" +
            $"{nameof(diagonalUpLeft)} => {diagonalUpLeft.ToVisualString()}\n" +
            $"{nameof(diagonalUpRight)} => {diagonalUpRight.ToVisualString()}\n" +
            $"{nameof(diagonalDownLeft)} => {diagonalDownLeft.ToVisualString()}\n" +
            $"{nameof(diagonalDownRight)} => {diagonalDownRight.ToVisualString()}\n");

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
