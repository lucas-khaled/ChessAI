using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TaskedPerftFunction : PerftFunction
{
    protected int currentDepth;
    protected bool isPerfting;

    public override async Task<long> Perft(int depth) 
    {
        if (isPerfting)
        {
            return -1;
        }

        isPerfting = true;
        currentDepth = depth;

        var data = await Task.Run(PerftTask);

        isPerfting = false;

        return data;
    }

    protected virtual async Task<long> PerftTask() 
    {
        return await Perft(currentDepth, manager.TestBoard);
    }

    protected virtual async Task<long> Perft(int depth, Board board) 
    {
        var moves = new List<Move>(board.actualTurnMoves);

        if (depth == 1)
            return moves.Count;

        long data = 0;

        if (moves == null || moves.Count <= 0)
            return 1;

        foreach(var move in moves)
        {
            DoMove(move, board);

            data += await Perft(depth - 1, board);

            UndoLastMove(board);
        }


        return data;
    }

    protected virtual void DoMove(Move move, Board board) 
    {
        manager.TurnManager.DoMove(move, board);
    }

    protected virtual void UndoLastMove(Board board)
    {
        manager.TurnManager.UndoLastMove(board);
    }
}
