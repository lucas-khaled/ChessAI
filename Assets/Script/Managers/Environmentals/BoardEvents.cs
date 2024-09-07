using System;

public class BoardEvents
{
    public Action<Move> onMoveMade;
    public Action<Move> onMoveUnmade;
    public Action<PromotionMove> onPromotionMade;
    public Action<PromotionMove> onPromotionUnmade;
    public Action<Piece> onPieceCaptured;
    public Action<Piece> onPieceUncaptured;
    public Action<PieceColor> onTurnDone;
    public Action<PieceColor> onTurnUndone;
}
