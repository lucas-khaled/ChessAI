using System.Collections.Generic;

public abstract class PinnerPiece : SlidingPieces
{
    public Bitboard KingDangerSquares { get; protected set; } = new Bitboard();
    public Piece Pinning { get; protected set; }

    protected PinnerPiece(Board board) : base(board)
    {
    }

    public override void GenerateBitBoard()
    {
        Pinning = null;
        KingDangerSquares.Clear();
        base.GenerateBitBoard();
    }

    protected List<Tile> GetKingDangerValidSegment(List<TileCoordinates> segment)
    {
        List<Tile> finalTiles = new();
        bool hasKing = false;
        Piece enemieInBetween = null;

        foreach (var tileCoord in segment)
        {
            Tile tile = Board.tiles[tileCoord.row][tileCoord.column];

            if (tile.IsOccupied)
            {
                if (tile.OccupiedBy.pieceColor == pieceColor) break;

                if (tile.OccupiedBy is King)
                {
                    hasKing = true;

                    if (enemieInBetween == null)
                        finalTiles.Add(tile);
                    continue;
                }
                
                if (enemieInBetween != null) break;

                enemieInBetween = tile.OccupiedBy;
                finalTiles.Add(tile);
                continue;
            }

            if(enemieInBetween == null)
                finalTiles.Add(tile);
        }

        if (hasKing) 
        {
            if (enemieInBetween != null)
            {
                Pinning = enemieInBetween;
                Pinning.PinnedBy = this;
            }
            return finalTiles;
        }

        return null;
    }
}
