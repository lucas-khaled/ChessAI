public class CastleMove : Move
{
    public Move rookMove;
    public CastleMove(Tile from, Tile to, King king, Move rookMove) : base(from, to, king)
    {
        this.rookMove = rookMove;
    }

    public override Move VirtualizeTo(Environment env)
    {
        var toTile = VirtualizeTile(to, env);
        var fromTile = VirtualizeTile(from, env);

        return new CastleMove(fromTile, toTile, VirtualizePiece(piece, env, toTile) as King, rookMove.VirtualizeTo(env));
    }
}
