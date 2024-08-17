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
        string captureString = (capture != null) ? capture.GetType().Name : "None";
        return $"Move Piece {piece.GetType().Name} {piece.pieceColor}" +
            $"\n - From tile ({from.TilePosition.row + 1}, {from.TilePosition.column + 1})" +
            $"\n - To tile ({to.TilePosition.row + 1}, {to.TilePosition.column + 1})" +
            $"\n - Capturing Piece {captureString}";
    }
}
