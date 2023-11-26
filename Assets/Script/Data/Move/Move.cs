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
        return new Move(VirtualizeTile(from, env), VirtualizeTile(to, env), VirtualizePiece(piece, env), VirtualizePiece(capture, env));
    }

    protected Tile VirtualizeTile(Tile tile, Environment env) 
    {
        var row = tile.TilePosition.row;
        var column = tile.TilePosition.column;

        return env.board.GetTiles()[row][column];
    }

    protected Piece VirtualizePiece(Piece piece, Environment env) 
    {
        return (piece == null) ? null : piece.Copy(env) as Piece;
    }
}
