public class CastleMove : Move
{
    public Move rookMove;
    public CastleMove(Tile from, Tile to, King king, Move rookMove) : base(from, to, king)
    {
        this.rookMove = rookMove;
    }

    public override Move VirtualizeTo(Board board)
    {
        var toTile = VirtualizeTile(to, board);
        var fromTile = VirtualizeTile(from, board);

        return new CastleMove(fromTile, toTile, fromTile.OccupiedBy as King, rookMove.VirtualizeTo(board));
    }

    public override string ToString()
    {
        return $"Castle Move Piece {piece}" +
            $"\n - From tile ({from.TilePosition})" +
            $"\n - To tile ({to.TilePosition})" +
            $"\n - Rook move:\n{rookMove}";
    }
}
