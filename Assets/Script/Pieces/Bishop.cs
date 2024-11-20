using System.Collections.Generic;

public class Bishop : SlidingPieces
{
    public Bishop(Board board) : base(board)
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

        kingDangerTiles.AddRange(GetTilesFromCoordinates(diagonals.topRightDiagonals));
        kingDangerTiles.AddRange(GetTilesFromCoordinates(diagonals.topLeftDiagonals));
        kingDangerTiles.AddRange(GetTilesFromCoordinates(diagonals.downRightDiagonals));
        kingDangerTiles.AddRange(GetTilesFromCoordinates(diagonals.downLeftDiagonals));

        KingDangerSquares = AddTilesBitBoards(kingDangerTiles);
    }

    private void GenerateAttackingSquaresBitBoard()
    {
        List<Tile> attackingTiles = new List<Tile>();

        attackingTiles.AddRange(GetDiagonalBlockedSquares());

        AttackingSquares = AddTilesBitBoards(attackingTiles);
    }

    public override Move[] GetMoves()
    {
        return GetDiagonalMoves().ToArray();
    }
}
