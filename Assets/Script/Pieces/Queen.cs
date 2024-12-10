using System.Collections.Generic;

public class Queen : SlidingPieces
{
    public Queen(Board board) : base(board)
    {
    }

    public override void GenerateBitBoard()
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

        var backVerticalDanger = GetTilesIfThereIsKing(verticals.backVerticals);
        if (backVerticalDanger != null)
            kingDangerTiles.AddRange(backVerticalDanger);

        var frontVerticalDanger = GetTilesIfThereIsKing(verticals.frontVerticals);
        if (frontVerticalDanger != null)
            kingDangerTiles.AddRange(frontVerticalDanger);

        var leftHorizontalDanger = GetTilesIfThereIsKing(horizontals.leftHorizontals);
        if (frontVerticalDanger != null)
            kingDangerTiles.AddRange(leftHorizontalDanger);

        var rightHorizontalDanger = GetTilesIfThereIsKing(horizontals.rightHorizontals);
        if (rightHorizontalDanger != null)
            kingDangerTiles.AddRange(rightHorizontalDanger);

        var topRightDanger = GetTilesIfThereIsKing(diagonals.topRightDiagonals);
        if (topRightDanger != null)
            kingDangerTiles.AddRange(topRightDanger);

        var topLeftDanger = GetTilesIfThereIsKing(diagonals.topLeftDiagonals);
        if (topLeftDanger != null)
            kingDangerTiles.AddRange(GetTilesFromCoordinates(diagonals.topLeftDiagonals));

        var downRightDanger = GetTilesIfThereIsKing(diagonals.downRightDiagonals);
        if (downRightDanger != null)
            kingDangerTiles.AddRange(GetTilesFromCoordinates(diagonals.downRightDiagonals));

        var downLeftDanger = GetTilesIfThereIsKing(diagonals.downLeftDiagonals);
        if (downLeftDanger != null)
            kingDangerTiles.AddRange(GetTilesFromCoordinates(diagonals.downLeftDiagonals));

        KingDangerSquares = AddTilesBitBoards(kingDangerTiles);
    }

    private void GenerateAttackingSquaresBitBoard()
    {
        List<Tile> attackingTiles = new List<Tile>();

        attackingTiles.AddRange(GetDiagonalBlockedSquares());
        attackingTiles.AddRange(GetVerticalBlockedSquares());
        attackingTiles.AddRange(GetHorizontalBlockedSquares());

        AttackingSquares = AddTilesBitBoards(attackingTiles);
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
