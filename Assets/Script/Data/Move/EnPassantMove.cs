public class EnPassantMove : Move
{
    public Tile capturedTile;
    public EnPassantMove(Tile from, Tile to, Piece piece, Piece capture) : base(from, to, piece, capture)
    {
        capturedTile = capture.GetTile();
    }
}
