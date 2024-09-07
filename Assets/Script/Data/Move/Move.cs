public class Move 
{
    public Tile from;
    public Tile to;
    public Piece capture;
    public Piece piece;

    public Move(Tile from, Tile to, Piece piece, Piece capture = null)
    {
        this.from = from;
        this.to = to;
        this.capture = capture;
        this.piece = piece;
    }

    public virtual Move VirtualizeTo(Board board) 
    {
        var toTile = VirtualizeTile(to, board);
        var fromTile = VirtualizeTile(from, board);
        var captureTile = (capture == null) ? null : VirtualizeTile(capture.GetTile(), board);
        var virtualizedPiece = fromTile.OccupiedBy;

        var virtualizedCapturePiece = (captureTile == null) ? null : captureTile.OccupiedBy;

        return new Move(fromTile, toTile, virtualizedPiece, virtualizedCapturePiece);
    }

    protected Tile VirtualizeTile(Tile tile, Board board) 
    {
        var row = tile.TilePosition.row;
        var column = tile.TilePosition.column;

        return board.GetTiles()[row][column];
    }

    protected Piece VirtualizePiece(Piece piece, Tile tile) 
    {
        return (piece == null) ? null : piece.Copy(tile);
    }

    public override string ToString()
    {
        return $"Move Piece {piece}" +
            $"\n - From tile ({from.TilePosition})" +
            $"\n - To tile ({to.TilePosition})" +
            $"\n - Capturing Piece {capture}";
    }

    public override bool Equals(object obj)
    {
        if (obj is not Move otherMove) return false;

        bool sameCapture = (capture == null) ? otherMove.capture == null : otherMove.capture.Equals(capture);
        return otherMove.from.Equals(from) && otherMove.to.Equals(to) 
            && sameCapture && piece.Equals(otherMove.piece);
    }
}
