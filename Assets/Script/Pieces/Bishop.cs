using System.Collections.Generic;
using UnityEngine.Profiling;

public class Bishop : PinnerPiece
{
    public Bishop(Board board) : base(board)
    {
    }

    protected override void GenerateBitBoardMethod()
    {
        Profiler.BeginSample("Move Generation > Generate Bitboard -> Bishop");
        GenerateAttackingSquaresBitBoard();
        GenerateKingDangerBitBoard();
        Profiler.EndSample();
    }

    private void GenerateKingDangerBitBoard()
    {
        var diagonals = actualTile.GetDiagonalsByColor(pieceColor);

        GeneratePinAndKingDangerBySegment(diagonals.topRightDiagonals);
        GeneratePinAndKingDangerBySegment(diagonals.topLeftDiagonals);
        GeneratePinAndKingDangerBySegment(diagonals.downRightDiagonals);
        GeneratePinAndKingDangerBySegment(diagonals.downLeftDiagonals);
    }

    private void GenerateAttackingSquaresBitBoard()
    {
        MovingSquares = AttackingSquares = actualTile.BishopLookup.GetBaseOnOccupancy(Board.moveGenerator.GetCurrentBoardBitboard());//GetDiagonalBlockedSquares();
    }
}
