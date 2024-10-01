using UnityEngine;

public class PromotionMove : Move
{
    public Piece promoteTo;

    public PromotionMove(Tile from, Tile to, Piece piece, Piece promoteTo, Piece capture = null) : base(from, to, piece, capture)
    {
        this.promoteTo = promoteTo;
    }

    public override Move VirtualizeTo(Board board)
    {
        Move virtualizedMove = base.VirtualizeTo(board);

        return new PromotionMove(virtualizedMove.from, virtualizedMove.to, virtualizedMove.piece, promoteTo, virtualizedMove.capture);
    }
}
