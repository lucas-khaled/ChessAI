using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
    public GameManager Manager { get; }

    public TurnManager(GameManager manager) 
    {
        Manager = manager;
    }

    #region DO
    public void DoMove(Move move, Board board)
    {
        if (IsValidMove(move) is false)
        {
            Debug.LogError($"The move is not valid:\n\n {move}");
            return;
        }

        Move convertedMove = ConvertMoveEnvironment(move, board);

        ComputeMove(convertedMove, board);

        SwapTurn(board);
        board.events?.onTurnDone?.Invoke(move.piece.pieceColor);
    }

    private Move ConvertMoveEnvironment(Move move, Board board)
    {
        if (move.from.Board != board || move.to.Board != board)
            return move.VirtualizeTo(board);

        return move;
    }

    private bool IsValidMove(Move move) 
    {
        return move.from.IsOccupied;
    }

    private void ComputeMove(Move move, Board board) 
    {
        EspecialRules oldRules = board.rules.Copy(board);
        string oldHash = board.ActualHash;

        if (move is CastleMove castleMove)
            ComputeCastleMove(castleMove, board);
        else if (move is PromotionMove promotionMove)
            ComputePromotionMove(promotionMove, board);
        else
            ComputeSimpleMove(move, board);

        board.events?.onMoveMade?.Invoke(move);

        
        long hash = Manager.HashManager.GetNewHashFromMove(Convert.ToInt64(oldHash), move, board.rules, oldRules);
        board.ActualHash = hash.ToString();

        SetTurn(move, board);
    }

    private void ComputeCastleMove(CastleMove move, Board board) 
    {
        ComputeSimpleMove(move, board);
        ComputeSimpleMove(move.rookMove, board);
    }

    private void ComputePromotionMove(PromotionMove move, Board board)
    {
        HandleCapture(move, board);

        Piece promotedPiece = move.promoteTo;
        Piece pawn = move.piece;

        promotedPiece.pieceColor = pawn.pieceColor;
        promotedPiece.SetTile(move.to);
        move.to.Occupy(promotedPiece);

        move.from.DeOccupy();

        board.pieces.Remove(pawn);
        board.pieces.Add(promotedPiece);

        if(pawn.pieceColor == PieceColor.White)
        {
            board.whitePieces.Add(promotedPiece);
            board.whitePieces.Remove(pawn);
        }
        else
        {
            board.blackPieces.Add(promotedPiece);
            board.blackPieces.Remove(pawn);
        }

        board.events?.onPromotionMade?.Invoke(move);
    }

    private void ComputeSimpleMove(Move move, Board board) 
    {
        Piece movingPiece = move.piece;
        move.from.DeOccupy();

        HandleCapture(move, board);

        move.to.Occupy(movingPiece);

        movingPiece.SetTile(move.to);
    }

    private void HandleCapture(Move move, Board board) 
    {
        var capturedPiece = move.capture;
        if (capturedPiece != null)
        {
            capturedPiece.GetTile().DeOccupy();

            Predicate<Piece> predicate = p => p.name.Equals(capturedPiece.name);
            board.pieces.RemoveAll(predicate);
            if (capturedPiece.pieceColor == PieceColor.White)
                board.whitePieces.RemoveAll(predicate);
            else
                board.blackPieces.RemoveAll(predicate);

            board.events?.onPieceCaptured?.Invoke(capturedPiece);
        }
    }

    private void SetTurn(Move move, Board board) 
    {
        Turn lastTurn = board.LastTurn;
        int halfMoves = (move.capture != null) ? 0 : lastTurn.halfMoves + 1;
        int fullMoves = (board.ActualTurn == PieceColor.Black) ? lastTurn.fullMoves + 1 : lastTurn.fullMoves;

        var turn = new Turn(move, board.ActualHash, halfMoves, fullMoves);
        board.turns.Add(turn);
    }
    #endregion

    #region UNDO
    public void UndoLastMove(Board board) 
    {
        EspecialRules oldRules = board.rules.Copy(board);
        Turn lastTurn = board.LastTurn;
        string hash = lastTurn.zobristHash;

        board.turns.Remove(lastTurn);

        Move lastMove = lastTurn.move;
        if (lastMove is CastleMove castleMove)
            UndoCastleMove(castleMove, board);
        else if (lastMove is PromotionMove promotionMove)
            UndoPromotionMove(promotionMove, board);
        else
            UndoSimpleMove(lastMove, board);

        board.events.onMoveUnmade?.Invoke(lastMove);

        board.ActualHash = hash;

        SwapTurn(board);
        board.events.onTurnUndone?.Invoke(lastMove.piece.pieceColor);
    }

    private void UndoCastleMove(CastleMove castleMove, Board board)
    {
        UndoSimpleMove(castleMove, board);
        UndoSimpleMove(castleMove.rookMove, board);
    }

    private void UndoPromotionMove(PromotionMove move, Board board)
    {
        move.to.DeOccupy();
        UndoCapture(move, board);

        Piece promotedPiece = move.promoteTo;
        Piece pawn = move.piece;

        promotedPiece.SetTile(null);
        move.from.Occupy(pawn);
        pawn.SetTile(move.from);

        board.pieces.Add(pawn);
        board.pieces.Remove(promotedPiece);

        if (pawn.pieceColor == PieceColor.White)
        {
            board.whitePieces.Remove(promotedPiece);
            board.whitePieces.Add(pawn);
        }
        else
        {
            board.blackPieces.Remove(promotedPiece);
            board.blackPieces.Add(pawn);
        }

        board.events.onPromotionUnmade?.Invoke(move);
    }

    private void UndoSimpleMove(Move lastMove, Board board)
    {
        lastMove.from.Occupy(lastMove.piece);
        lastMove.piece.SetTile(lastMove.from);
        lastMove.to.DeOccupy();

        UndoCapture(lastMove, board);
    }

    private void UndoCapture(Move move, Board board) 
    {
        Piece capturedPiece = move.capture;
        if (capturedPiece != null)
        {
            move.to.Occupy(capturedPiece);
            capturedPiece.SetTile(move.to);

            board.pieces.Add(capturedPiece);

            if (capturedPiece.pieceColor == PieceColor.White)
                board.whitePieces.Add(capturedPiece);
            else
                board.blackPieces.Add(capturedPiece);

            board.events.onPieceUncaptured?.Invoke(capturedPiece);
        }
    }
    #endregion

    private void SwapTurn(Board board)
    {
        var thisTurn = board.ActualTurn;
        board.ActualTurn = (thisTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;
    }

    public void DebugAllTurns(Board board) 
    {
        int count = 0;
        foreach (var turn in board.turns)
        {
            count++;
            Debug.Log($"Turn {count}:\n{turn}");
        }
    }
}

public struct Turn 
{
    public Move move;
    public string zobristHash;
    public int halfMoves;
    public int fullMoves; 

    public Turn(Move move, string zobristHash, int halfMoves, int fullMoves) 
    {
        this.move = move;
        this.zobristHash = zobristHash;
        this.halfMoves = halfMoves;
        this.fullMoves = fullMoves;
    }

    public override string ToString()
    {
        return $"{move};\n\nFEN: {zobristHash}";
    }
}
