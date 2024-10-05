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

    public override string ToString()
    {
        return $"Promotion Move Piece {piece}" +
            $"\n - From tile ({from.TilePosition})" +
            $"\n - To tile ({to.TilePosition})" +
            $"\n - Capturing Piece {capture}"+
            $"\n - Promoting to {promoteTo}";
    }
}
