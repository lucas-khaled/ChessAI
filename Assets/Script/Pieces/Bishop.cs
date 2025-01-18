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
        List<Tile> attackingTiles = new List<Tile>();

        attackingTiles.AddRange(GetDiagonalBlockedSquares());

        MovingSquares = AttackingSquares = AddTilesBitBoards(attackingTiles);
    }

    public override Move[] GetMoves()
    {
        return GetDiagonalMoves().ToArray();
    }
}
