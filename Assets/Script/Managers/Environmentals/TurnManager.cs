using System.Collections.Generic;
using UnityEngine;

public class TurnManager : IEnvironmentable
{
    public PieceColor ActualTurn { get; set; } = PieceColor.White;

    public List<Turn> moves { get; private set; } = new List<Turn>();
    public int halfMoves = 0;
    public int fullMoves = 0;

    public Environment Environment { get; }
    private FENManager FENManager;

    public TurnManager(Environment env) 
    {
        Environment = env;
        FENManager = new FENManager(env);
    }

    public IEnvironmentable Copy(Environment env)
    {
        return new TurnManager(env)
        {
            moves = new List<Turn>(moves),
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

        IncrementNumberOfMoves();
        ComputeMove(convertedMove);

        var thisTurn = ActualTurn;

        ActualTurn = (thisTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;
        Environment.events?.onTurnDone?.Invoke(thisTurn);
    }

    private void IncrementNumberOfMoves()
    {
        halfMoves++;
        if (ActualTurn == PieceColor.Black)
            fullMoves++;
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

        moves.Add(new Turn(move, FENManager.GetFEN()));

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
        var capturedPiece = move.capture;
        if (capturedPiece != null)
        {
            capturedPiece.GetTile().DeOccupy();
            halfMoves = 0;
            Environment.board.pieces.Remove(capturedPiece);
            this.Environment.events?.onPieceCaptured?.Invoke(capturedPiece);
        }
    }

    public void DebugAllTurns() 
    {
        int count = 0;
        foreach (var turn in moves)
        {
            count++;
            Debug.Log($"Turn {count}:\n{turn}");
        }
    }
}

public struct Turn 
{
    public Move move;
    public FEN fen;

    public Turn(Move move, FEN fen) 
    {
        this.move = move;
        this.fen = fen;
    }

    public override string ToString()
    {
        return $"{move};\n\nFEN: {fen}";
    }
}
