public abstract class PinnerPiece : SlidingPieces
{
    public Bitboard KingDangerSquares { get; set; }

    protected PinnerPiece(Board board) : base(board)
    {
    }
}
