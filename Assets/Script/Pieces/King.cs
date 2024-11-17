using System.Collections.Generic;

public class King : SlidingPieces
{
    private CheckChecker checkChecker = new();

    public King(Board board) : base(board)
    {
        
    }

    public override Move[] GetMoves()
    {
        List<Move> moves = new List<Move>();

        var horizontal = GetHorizontalMoves(1);
        var vertical = GetVerticalMoves(1);
        var diagonals = GetDiagonalMoves(1);

        moves.AddRange(horizontal);
        moves.AddRange(vertical);
        moves.AddRange(diagonals);
        moves.AddRange(GetCastleMoves());

        return moves.ToArray();
    }

    private List<CastleMove> GetCastleMoves() 
    {
        List<CastleMove> moves = new List<CastleMove>();


        if (Board.rules.HasCastledAllSides(pieceColor) || checkChecker.IsCheck(Board, pieceColor)) return moves;
        
        var tiles = Board.GetRookTiles(pieceColor);

        foreach(var tile in tiles) 
            CheckRook(tile, ref moves);

        return moves;
    }

    private void CheckRook(Tile tile, ref List<CastleMove> moves) 
    {
        if (Board.rules.CanCastle(pieceColor, tile.OccupiedBy as Rook) is false) return;

        int range = tile.TilePosition.column - Column;

        bool isWhite = pieceColor == PieceColor.White;
        bool positiveRange = range > 0;
        bool isRight = (!isWhite || positiveRange) && (!positiveRange || isWhite);

        var inBetweenTiles = isRight ?
            actualTile.GetHorizontalsByColor(pieceColor).rightHorizontals:
            actualTile.GetHorizontalsByColor(pieceColor).leftHorizontals;

        var blockingCheckedBetween = CheckForBlockingSquares(inBetweenTiles, includeBlockingPieceSquare: true);

        if (blockingCheckedBetween[blockingCheckedBetween.Count - 1].TilePosition.Equals(tile.TilePosition))
            moves.Add(CreateCastleMove(tile, range));
    }

    private CastleMove CreateCastleMove(Tile rookTile, int range) 
    {
        var iterator = (range > 0) ? 1 : -1;

        Tile rookToTile = Board.GetTiles()[Row][Column + iterator]; 
        Move rookMove = new Move(rookTile, rookToTile, rookTile.OccupiedBy);

        Tile toTile = Board.GetTiles()[Row][Column + iterator * 2];

        return new CastleMove(actualTile, toTile, this, rookMove);
    }

    public override void GenerateBitBoard()
    {
        throw new System.NotImplementedException();
    }
}
