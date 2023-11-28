public class CastleMove : Move
{
    public Move rookMove;
    public CastleMove(Tile from, Tile to, King king, Move rookMove) : base(from, to, king)
    {
        this.rookMove = rookMove;
    }

    public override Move VirtualizeTo(Environment env)
    {
        return new CastleMove(VirtualizeTile(from, env), VirtualizeTile(to, env), VirtualizePiece(piece, env) as King, rookMove.VirtualizeTo(env));
    }
}
