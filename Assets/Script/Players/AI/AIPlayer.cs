using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    protected List<Move> GetAllMoves(Environment environment, PieceColor color)
    {
        List<Move> possibleMoves = new List<Move>();
        foreach (var piece in environment.board.pieces.Where(x => x.pieceColor == color))
        {
            var moves = environment.moveMaker.GetMoves(piece);
            possibleMoves.AddRange(moves);
        }

        return possibleMoves;
    }

    protected abstract Task<Move> CalculateMove();
}
