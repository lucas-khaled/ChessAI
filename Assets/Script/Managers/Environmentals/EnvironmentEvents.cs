using System;

public class EnvironmentEvents
{
    public Action<Move> onMoveMade;
    public Action<PromotionMove> onPromotionMade;
    public Action<Piece> onPieceCaptured;
    public Action<PieceColor> onTurnDone;
}
