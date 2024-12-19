using System.Collections.Generic;

public class Bishop : PinnerPiece
{
    public Bishop(Board board) : base(board)
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

        var topRightDanger = GetKingDangerValidSegment(diagonals.topRightDiagonals);
        if(topRightDanger != null)
            kingDangerTiles.AddRange(topRightDanger);

        var topLeftDanger = GetKingDangerValidSegment(diagonals.topLeftDiagonals);
        if(topLeftDanger != null)
            kingDangerTiles.AddRange(topLeftDanger);

        var downRightDanger = GetKingDangerValidSegment(diagonals.downRightDiagonals);
        if(downRightDanger != null)
            kingDangerTiles.AddRange(downRightDanger);

        var downLeftDanger = GetKingDangerValidSegment(diagonals.downLeftDiagonals);
        if(downLeftDanger != null)
            kingDangerTiles.AddRange(downLeftDanger);

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
