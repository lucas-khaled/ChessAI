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

        var topRightDanger = GetKingDangerValidSegment(diagonals.topRightDiagonals);
        if(topRightDanger != null)
            kingDangerTiles.AddRange(topRightDanger);

        var topLeftDanger = GetKingDangerValidSegment(diagonals.topLeftDiagonals);
        if(topLeftDanger != null)
            kingDangerTiles.AddRange(GetTilesFromCoordinates(diagonals.topLeftDiagonals));

        var downRightDanger = GetKingDangerValidSegment(diagonals.downRightDiagonals);
        if(downRightDanger != null)
            kingDangerTiles.AddRange(GetTilesFromCoordinates(diagonals.downRightDiagonals));

        var downLeftDanger = GetKingDangerValidSegment(diagonals.downLeftDiagonals);
        if(downLeftDanger != null)
            kingDangerTiles.AddRange(GetTilesFromCoordinates(diagonals.downLeftDiagonals));

        KingDangerSquares = AddTilesBitBoards(kingDangerTiles);
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
