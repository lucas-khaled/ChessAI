using System.Collections.Generic;

public class Rook : PinnerPiece
{
    public Rook(Board board) : base(board)
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
        
        var verticals = actualTile.GetVerticalsByColor(pieceColor);
        var horizontals = actualTile.GetHorizontalsByColor(pieceColor);

        var backVerticalDanger = GetKingDangerValidSegment(verticals.backVerticals);
        if(backVerticalDanger != null)
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

        KingDangerSquares = AddTilesBitBoards(kingDangerTiles);
    }

    private void GenerateAttackingSquaresBitBoard() 
    {
        List<Tile> attackingTiles = new List<Tile>();

        attackingTiles.AddRange(GetVerticalBlockedSquares());
        attackingTiles.AddRange(GetHorizontalBlockedSquares());

        MovingSquares = AttackingSquares = AddTilesBitBoards(attackingTiles);
    }

    public override Move[] GetMoves()
    {
        List<Move> moves = new();
        moves.AddRange(GetVerticalMoves());
        moves.AddRange(GetHorizontalMoves());

        return moves.ToArray();
    }
}
