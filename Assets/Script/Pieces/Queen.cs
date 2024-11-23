using System.Collections.Generic;

public class Queen : PinnerPiece
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

        kingDangerTiles.AddRange(GetTilesFromCoordinates(diagonals.topRightDiagonals));
        kingDangerTiles.AddRange(GetTilesFromCoordinates(diagonals.topLeftDiagonals));
        kingDangerTiles.AddRange(GetTilesFromCoordinates(diagonals.downRightDiagonals));
        kingDangerTiles.AddRange(GetTilesFromCoordinates(diagonals.downLeftDiagonals));

        kingDangerTiles.AddRange(GetTilesFromCoordinates(verticals.backVerticals));
        kingDangerTiles.AddRange(GetTilesFromCoordinates(verticals.frontVerticals));
        kingDangerTiles.AddRange(GetTilesFromCoordinates(horizontals.leftHorizontals));
        kingDangerTiles.AddRange(GetTilesFromCoordinates(horizontals.rightHorizontals));

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
