using UnityEngine;

public class PromotionMove : Move
{
    public Piece promoteTo;

    public PromotionMove(Tile from, Tile to, Piece piece, Piece promoteTo, Piece capture = null) : base(from, to, piece, capture)
    {
        this.promoteTo = promoteTo;
    }
}
