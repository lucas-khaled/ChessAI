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

    protected List<Move> SortMoveWithLinq(List<Move> moves) 
    {
        moves.Sort(Compare);

        return moves;
    }

    protected List<Move> SortMoves(List<Move> moves) 
    {
        int[] moveHeuristics = new int[moves.Count];
        for(int i = 0; i < moves.Count; i++) 
        {
            var move = moves[i];
            var heuristic = GetMoveHeuristic(move);
            moveHeuristics[i] = heuristic;
        }

        var finalMoves = CountingSortMoves(moveHeuristics, moves);
        return finalMoves;
    }

    private int Compare(Move x, Move y) 
    {
        var xHeuristic = GetMoveHeuristic(x);
        var yHeuristic = GetMoveHeuristic(y);

        return yHeuristic - xHeuristic; 
    }

    private int GetMoveHeuristic(Move move) 
    {
        var heuristic = 0;

        var capture = move.capture;
        if (capture != null)
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

        return heuristic;
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
            --count[moveHeuristics[i]];
            var index = moves.Count - count[moveHeuristics[i]] - 1;
            outputMoves[index] = moves[i];
        }

        return outputMoves.ToList();
    }

    protected abstract Task<Move> CalculateMove();

    protected struct MoveSortHeuristic
    {
        public Move move;
        public int heuristic;
    }
}
