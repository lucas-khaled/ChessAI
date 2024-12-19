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
        List<Tile> kingDangerTiles = new List<Tile>();

        var diagonals = actualTile.GetDiagonalsByColor(pieceColor);
        var verticals = actualTile.GetVerticalsByColor(pieceColor);
        var horizontals = actualTile.GetHorizontalsByColor(pieceColor);

        var backVerticalDanger = GetKingDangerValidSegment(verticals.backVerticals);
        if (backVerticalDanger != null)
            kingDangerTiles.AddRange(backVerticalDanger);

        var frontVerticalDanger = GetKingDangerValidSegment(verticals.frontVerticals);
        if (frontVerticalDanger != null)
            kingDangerTiles.AddRange(frontVerticalDanger);

        var leftHorizontalDanger = GetKingDangerValidSegment(horizontals.leftHorizontals);
        if (leftHorizontalDanger != null)
            kingDangerTiles.AddRange(leftHorizontalDanger);

        var rightHorizontalDanger = GetKingDangerValidSegment(horizontals.rightHorizontals);
        if (rightHorizontalDanger != null)
            kingDangerTiles.AddRange(rightHorizontalDanger);

        var topRightDanger = GetKingDangerValidSegment(diagonals.topRightDiagonals);
        if (topRightDanger != null)
            kingDangerTiles.AddRange(topRightDanger);

        var topLeftDanger = GetKingDangerValidSegment(diagonals.topLeftDiagonals);
        if (topLeftDanger != null)
            kingDangerTiles.AddRange(topLeftDanger);

        var downRightDanger = GetKingDangerValidSegment(diagonals.downRightDiagonals);
        if (downRightDanger != null)
            kingDangerTiles.AddRange(downRightDanger);

        var downLeftDanger = GetKingDangerValidSegment(diagonals.downLeftDiagonals);
        if (downLeftDanger != null)
            kingDangerTiles.AddRange(downLeftDanger);

        KingDangerSquares = AddTilesBitBoards(kingDangerTiles);
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
