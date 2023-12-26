using System.Collections.Generic;
using UnityEngine;

public class TurnManager : IEnvironmentable
{
    public PieceColor ActualTurn { get; set; } = PieceColor.White;

    public List<Move> moves  { get; private set; } = new List<Move>();

    public Move LastMove => (moves.Count > 0) ? moves[moves.Count - 1] : null;

    public Environment Environment { get; }

    public TurnManager(Environment env) 
    {
        Environment = env;
    }

    public IEnvironmentable Copy(Environment env)
    {
        return new TurnManager(env)
        {
            moves = new List<Move>(moves),
            ActualTurn = ActualTurn
        };
    }

    public void DoMove(Move move) 
    {
        if(IsValidMove(move) is false) 
        {
            Debug.LogError($"The move is not valid");
            return;
        }

        Move convertedMove = ConvertMoveEnvironment(move);

        ComputeMove(convertedMove);

        var thisTurn = ActualTurn;
        ActualTurn = (thisTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;
        Environment.events?.onTurnDone?.Invoke(thisTurn);
    }

    private Move ConvertMoveEnvironment(Move move)
    {
        if (move.from.Environment != this.Environment || move.to.Environment != this.Environment)
            return move.VirtualizeTo(Environment);

        return move;
    }

    private bool IsValidMove(Move move) 
    {
        return move.from.IsOccupied;
    }

    private void ComputeMove(Move move) 
    {
        if (move is CastleMove castleMove)
            ComputeCastleMove(castleMove);
        else if (move is PromotionMove promotionMove)
            ComputePromotionMove(promotionMove);
        else
            ComputeSimpleMove(move);

        moves.Add(move);

        this.Environment.events?.onMoveMade?.Invoke(move);
    }

    private void ComputeCastleMove(CastleMove move) 
    {
        ComputeSimpleMove(move);
        ComputeSimpleMove(move.rookMove);
    }

    private void ComputePromotionMove(PromotionMove move)
    {
        move.promoteTo.pieceColor = move.piece.pieceColor;
        move.promoteTo.SetTile(move.to);
        move.to.Occupy(move.promoteTo);

        move.from.DeOccupy();

        HandleCapture(move);

        Environment.events?.onPromotionMade?.Invoke(move);
    }

    private void ComputeSimpleMove(Move move) 
    {
        Piece movingPiece = move.piece;
        move.from.DeOccupy();

        HandleCapture(move);

        move.to.Occupy(movingPiece);

        movingPiece.SetTile(move.to);
    }

    private void HandleCapture(Move move) 
    {
        if (move.capture != null)
        {
            move.capture.GetTile().DeOccupy();
            this.Environment.events?.onPieceCaptured?.Invoke(move.capture);
        }
    }
}
