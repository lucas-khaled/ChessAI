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

    public virtual Move VirtualizeTo(Environment env) 
    {
        var toTile = VirtualizeTile(to, env);
        var fromTile = VirtualizeTile(from, env);
        var captureTile = (capture == null) ? null : VirtualizeTile(capture.GetTile(), env);

        return new Move(fromTile, toTile, VirtualizePiece(piece, env, toTile), VirtualizePiece(capture, env, captureTile));
    }

    protected Tile VirtualizeTile(Tile tile, Environment env) 
    {
        var row = tile.TilePosition.row;
        var column = tile.TilePosition.column;

        return env.board.GetTiles()[row][column];
    }

    protected Piece VirtualizePiece(Piece piece, Environment env, Tile tile) 
    {
        return (piece == null) ? null : piece.Copy(env, tile) as Piece;
    }
}
