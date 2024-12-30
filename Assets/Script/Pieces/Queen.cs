using System.Collections.Generic;

public class Queen : PinnerPiece
{
    public Queen(Board board) : base(board)
    {
    }

    protected override void GenerateBitBoardMethod()
    {
        GenerateAttackingSquaresBitBoard();
        GenerateKingDangerBitBoard();
    }

    private void GenerateKingDangerBitBoard()
    {
        var diagonals = actualTile.GetDiagonalsByColor(pieceColor);
        var verticals = actualTile.GetVerticalsByColor(pieceColor);
        var horizontals = actualTile.GetHorizontalsByColor(pieceColor);

        GeneratePinAndKingDangerBySegment(verticals.backVerticals);
        GeneratePinAndKingDangerBySegment(verticals.frontVerticals);
        GeneratePinAndKingDangerBySegment(horizontals.leftHorizontals);
        GeneratePinAndKingDangerBySegment(horizontals.rightHorizontals);
        GeneratePinAndKingDangerBySegment(diagonals.topRightDiagonals);
        GeneratePinAndKingDangerBySegment(diagonals.topLeftDiagonals);
        GeneratePinAndKingDangerBySegment(diagonals.downRightDiagonals);
        GeneratePinAndKingDangerBySegment(diagonals.downLeftDiagonals);
    }

    private void GenerateAttackingSquaresBitBoard()
    {
        List<Tile> attackingTiles = new List<Tile>();

        attackingTiles.AddRange(GetDiagonalBlockedSquares());
        attackingTiles.AddRange(GetVerticalBlockedSquares());
        attackingTiles.AddRange(GetHorizontalBlockedSquares());

        MovingSquares = AttackingSquares = AddTilesBitBoards(attackingTiles);
    }

    public override Move[] GetMoves()
    {
        List<Move> moves = new();

        moves.AddRange(GetDiagonalMoves());
        moves.AddRange(GetVerticalMoves());
        moves.AddRange(GetHorizontalMoves());

        return moves.ToArray();
    }
}
