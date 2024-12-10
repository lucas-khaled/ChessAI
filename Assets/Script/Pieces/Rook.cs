using System.Collections.Generic;

public class Rook : SlidingPieces
{
    public Rook(Board board) : base(board)
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
        
        var verticals = actualTile.GetVerticalsByColor(pieceColor);
        var horizontals = actualTile.GetHorizontalsByColor(pieceColor);

        var backVerticalDanger = GetTilesIfThereIsKing(verticals.backVerticals);
        if(backVerticalDanger != null)
            kingDangerTiles.AddRange(backVerticalDanger);

        var frontVerticalDanger = GetTilesIfThereIsKing(verticals.frontVerticals);
        if (frontVerticalDanger != null)
            kingDangerTiles.AddRange(frontVerticalDanger);

        var leftHorizontalDanger = GetTilesIfThereIsKing(horizontals.leftHorizontals);
        if (leftHorizontalDanger != null)
            kingDangerTiles.AddRange(leftHorizontalDanger);

        var rightHorizontalDanger = GetTilesIfThereIsKing(horizontals.rightHorizontals);
        if (rightHorizontalDanger != null)
            kingDangerTiles.AddRange(rightHorizontalDanger);

        KingDangerSquares = AddTilesBitBoards(kingDangerTiles);
    }

    private void GenerateAttackingSquaresBitBoard() 
    {
        List<Tile> attackingTiles = new List<Tile>();

        attackingTiles.AddRange(GetVerticalBlockedSquares());
        attackingTiles.AddRange(GetHorizontalBlockedSquares());

        AttackingSquares = AddTilesBitBoards(attackingTiles);
    }

    public override Move[] GetMoves()
    {
        List<Move> moves = new();
        moves.AddRange(GetVerticalMoves());
        moves.AddRange(GetHorizontalMoves());

        return moves.ToArray();
    }
}
