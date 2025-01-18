using System.Collections.Generic;
using UnityEngine.Profiling;

public class Rook : PinnerPiece
{
    public Rook(Board board) : base(board)
    {
    }

    protected override void GenerateBitBoardMethod()
    {
        Profiler.BeginSample("Move Generation > Generate Bitboard -> Rook");
        GenerateAttackingSquaresBitBoard();   
        GenerateKingDangerBitBoard();
        Profiler.EndSample();
    }

    private void GenerateKingDangerBitBoard()
    {
        var verticals = actualTile.GetVerticalsByColor(pieceColor);
        var horizontals = actualTile.GetHorizontalsByColor(pieceColor);

        GeneratePinAndKingDangerBySegment(verticals.backVerticals);
        GeneratePinAndKingDangerBySegment(verticals.frontVerticals);
        GeneratePinAndKingDangerBySegment(horizontals.leftHorizontals);
        GeneratePinAndKingDangerBySegment(horizontals.rightHorizontals);
    }

    private void GenerateAttackingSquaresBitBoard() 
    {
        List<Tile> attackingTiles = new List<Tile>();

        attackingTiles.AddRange(GetVerticalBlockedSquares());
        attackingTiles.AddRange(GetHorizontalBlockedSquares());

        MovingSquares = AttackingSquares = AddTilesBitBoards(attackingTiles);
    }
}
