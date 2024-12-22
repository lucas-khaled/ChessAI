using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PerftFunction : MonoBehaviour
{
    /*public Action<ulong, int> onDepthAnalized { get; set; }
    public Action onFinishedAnalizing { get; set; }*/

    private MoveGenerator generator;
    private Board board;
    private PerftManager manager;

    private object movesLock = new();
    private Queue<Move> doMoveList = new Queue<Move>();
    private int undoMoveCount = 0;

    private int currentDepth;
    private PieceColor currentColor;

    public void Initialize(Board board, PerftManager manager) 
    {
        this.board = board;
        generator = new MoveGenerator(board);
        this.manager = manager;
    }

    public async Task<ulong> Perft(int depth) 
    {
        InvokeRepeating("DoMoveTracking", 0, 0.1f);

        currentColor = board.ActualTurn;
        currentDepth = depth;
        ulong count = await Task.Run(PerftTask);

        CancelInvoke("DoMoveTracking");
        return count;
    }

    private void DoMoveTracking() 
    {
        lock (movesLock) 
        {
            if (doMoveList.Count > 0)
                manager.TurnManager.DoMove(doMoveList.Dequeue(), board);

            if(undoMoveCount > 0) 
            {
                manager.TurnManager.UndoLastMove(board);
                undoMoveCount--;
            }

        }
    }

    private async Task<ulong> PerftTask() 
    {
        return await Perft(currentDepth, currentColor);
    }

    private async Task<ulong> Perft(int depth, PieceColor color) 
    {
        if (depth <= 0)
            return 1;

        var moves = generator.GenerateMoves(color);
        if (moves == null || moves.Count <= 0) return 1;

        ulong nodes = 0;
        foreach(var move in moves)
        {
            AddMoveToQueue(move);
            nodes += await Perft(depth - 1, color.GetOppositeColor());
            AddUndoCount();
        }

        return nodes;
    }

    private void AddUndoCount()
    {
        lock (movesLock)
        {
            undoMoveCount++;
        }
    }

    private void AddMoveToQueue(Move move)
    {
        lock (movesLock)
        {
            doMoveList.Enqueue(move);
        }
    }
}
