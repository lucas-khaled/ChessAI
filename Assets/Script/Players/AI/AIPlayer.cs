using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

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

    protected List<Move> GetAllMoves(Board board, PieceColor color)
    {
        List<Move> possibleMoves = new List<Move>();
        List<Piece> pieces = (color == PieceColor.White) ? board.whitePieces : board.blackPieces;

        for(int i = 0; i < pieces.Count; i++)
        {
            var piece = pieces[i];
            var moves = manager.MoveChecker.GetMoves(piece);
            possibleMoves.AddRange(moves);
        }

        return possibleMoves;
    }

    protected List<Move> SortMoves(List<Move> moves) 
    {
        int[] moveHeuristics = new int[moves.Count];
        for(int i = 0; i < moves.Count; i++) 
        {
            var move = moves[i];
            var heuristic = 0;

            var capture = move.capture;
            if(capture != null) 
            {
                if (capture is Queen)
                    heuristic += 5;
                else if (capture is Rook)
                    heuristic += 4;
                else if (capture is Bishop || capture is Knight)
                    heuristic += 3;
                else if (capture is Pawn)
                    heuristic += 2;
            }

            if (move is CastleMove)
                heuristic += 1;

            if (move is PromotionMove)
                heuristic += 5;

            
            moveHeuristics[i] = heuristic;
        }

        return CountingSortMoves(moveHeuristics, moves);
    }

    private List<Move> CountingSortMoves(int[] moveHeuristics, List<Move> moves) 
    {
        if(moveHeuristics.Length != moves.Count) 
        {
            Debug.LogError("Move sort heuristics array are not the same length of moves array");
            return null;
        }

        int[] count = new int[11];
        Move[] outputMoves = new Move[moves.Count];

        for(int i = 0; i< moveHeuristics.Length; i++) 
        {
            count[moveHeuristics[i]]++;
        }

        for (int i = 1; i < count.Length; i++)
        {
            count[i] += count[i - 1];
        }

        for(int i = moveHeuristics.Length-1; i >= 0; i--) 
        {
            var index = --count[moveHeuristics[i]];
            outputMoves[index] = moves[i];
        }

        return outputMoves.ToList();
    }

    protected abstract Task<Move> CalculateMove();

    protected struct MoveDTO
    {
        public Move move;
        public bool WasCheckmate;
        public bool WasDraw;
    }
}
