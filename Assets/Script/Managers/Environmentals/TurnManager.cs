using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : IEnvironmentable
{
    public PieceColor ActualTurn { get; private set; } = PieceColor.White;

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
        if(CheckValidMove(move) is false) 
        {
            Debug.LogError($"The move is not valid");
            return;
        }

        Move convertedMove = ConvertMoveEnvironment(move);

        ComputeMove(convertedMove);
        
        ActualTurn = (ActualTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;
    }

    private Move ConvertMoveEnvironment(Move move)
    {
        if (move.from.Environment != this.Environment || move.to.Environment != this.Environment)
            return move.VirtualizeTo(Environment);

        return move;
    }

    private bool CheckValidMove(Move move) 
    {
        return move.from.IsOccupied;
    }

    private void ComputeMove(Move move) 
    {
        if (move is CastleMove castleMove)
            ComputeCastleMove(castleMove);
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

    private void ComputeSimpleMove(Move move) 
    {
        Piece movingPiece = move.piece;
        move.from.DeOccupy();

        if (move.capture != null)
            this.Environment.events?.onPieceCaptured?.Invoke(move.capture);

        move.to.Occupy(movingPiece);

        movingPiece.SetTile(move.to);
    }
}
