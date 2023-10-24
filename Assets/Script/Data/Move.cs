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

    public Move VirtualizeTo(Board board) 
    {
        var fromRow = from.TilePosition.row;
        var fromColumn = from.TilePosition.column;

        var toRow = to.TilePosition.row;
        var toColumn = to.TilePosition.column;
        return new Move(board.GetTiles()[fromRow][fromColumn], board.GetTiles()[toRow][toColumn], piece, capture);
    }
}

public class CastleMove : Move
{
    public Move rookMove;
    public CastleMove(Tile from, Tile to, King king, Move rookMove) : base(from, to, king)
    {
        this.rookMove = rookMove;
    }
}
