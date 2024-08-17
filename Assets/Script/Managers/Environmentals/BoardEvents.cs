using System;

public class BoardEvents
{
    public Action<Move> onMoveMade;
    public Action<Move> onMoveUndone;
    public Action<PromotionMove> onPromotionMade;
    public Action<Piece> onPieceCaptured;
    public Action<PieceColor> onTurnDone;
}
