using System;

public class BoardEvents
{
    public Action<PromotionMove> onPromotionMade;
    public Action<PromotionMove> onPromotionUnmade;
    public Action<Piece> onPieceCaptured;
    public Action<Piece> onPieceUncaptured;
    public Action<PieceColor> onTurnDone;
    public Action<PieceColor> onTurnUndone;
}
