using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIPlayer : Player
{
    private List<Piece> pieces;
    private object pieceLock = new();

    public AIPlayer(GameManager manager) : base(manager)
    {
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

    private async Task<Move> CalculateMove() 
    {
        await Task.Yield();
        var allMoves = GetAllMoves();
        int index = Random.Range(0, allMoves.Count);

        var move = allMoves[index];
        return move;
    }

    private List<Move> GetAllMoves() 
    {
        List<Move> possibleMoves = new List<Move>();
        lock (pieceLock)
        {
            foreach (var piece in pieces)
            {
                var moves = manager.environment.moveMaker.GetMoves(piece);
                possibleMoves.AddRange(moves);
            }
        }

        return possibleMoves;
    }
}
