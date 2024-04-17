using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

public abstract class AIPlayer : Player
{
    protected List<Piece> pieces;
    protected object pieceLock = new();
    protected float minimumWaitTime = 0;

    public AIPlayer(GameManager manager, float minimumWaitTime = 0) : base(manager)
    {
        this.minimumWaitTime = minimumWaitTime;
    }

    public override async void StartTurn(Action<Move> moveCallback)
    {
        base.StartTurn(moveCallback);
        Move move = await CalculateMove();
        onMove?.Invoke(move);
    }

    public override void Init(PieceColor pieceColor)
    {
        base.Init(pieceColor);

        pieces = manager.environment.board.pieces.Where(x => x.pieceColor == actualColor).ToList();
        manager.environment.events.onPromotionMade += PromotedMove;
        manager.environment.events.onPieceCaptured += CapturedPiece;
    }

    private void CapturedPiece(Piece piece) 
    {
        if (piece.pieceColor != actualColor) return;

        lock (pieceLock) 
        {
            pieces.Remove(piece);
        }
    }

    private void PromotedMove(PromotionMove promotion) 
    {
        if (promotion.piece.pieceColor != actualColor) return;

        lock (pieceLock)
        {
            pieces.Remove(promotion.piece);
            pieces.Add(promotion.promoteTo);
        }
    }

    protected abstract Task<Move> CalculateMove();
}
