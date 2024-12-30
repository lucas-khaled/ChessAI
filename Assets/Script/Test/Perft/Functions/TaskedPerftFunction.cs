using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class TaskedPerftFunction : PerftFunction
{
    protected int currentDepth;
    protected bool isPerfting;
    protected bool divide;

    protected Dictionary<string, long> divideDict = new Dictionary<string, long>();

    private Stopwatch doMoveTimer = new();
    private Stopwatch undoMoveTimer = new();
    private Stopwatch perftTimer = new();

    public override async Task<long> Perft(int depth, bool divide = true) 
    {
        if (isPerfting)
        {
            return -1;
        }

        perftTimer.Reset();
        doMoveTimer.Reset();
        undoMoveTimer.Reset();

        this.divide = divide;
        divideDict.Clear();

        isPerfting = true;
        currentDepth = depth;

        var data = await PerftTask();

        isPerfting = false;

        return data;
    }

    protected virtual async Task<long> PerftTask() 
    {
        UnityEngine.Debug.Log($"Starting Perft with depth {currentDepth}");

        perftTimer.Start();
        long result = await Perft(currentDepth, manager.TestBoard);
        perftTimer.Stop();

        long medianDoTime = doMoveTimer.ElapsedMilliseconds / result;
        long medianUndoTime = undoMoveTimer.ElapsedMilliseconds / result;

        UnityEngine.Debug.Log($"Finished Perft with depth {currentDepth}:\n " +
            $"Perft time: {perftTimer.ElapsedMilliseconds}ms\n" +
            $"Do time: {medianDoTime}ms\n" +
            $"Undo time: {medianUndoTime}ms\n");

        if (divide) 
            DebugDivide();

        return result;
    }

    private void DebugDivide()
    {
        string debugString = string.Empty;
        foreach(var keyValuePair in divideDict) 
        {
            debugString += $"{keyValuePair.Key}: {keyValuePair.Value}\n";
        }

        UnityEngine.Debug.Log("Divide:\n"+debugString);
    }

    protected virtual async Task<long> Perft(int depth, Board board) 
    {
        var moves = new List<Move>(board.currentTurnMoves);

        if (depth == 1)
            return moves.Count;

        if (moves == null || moves.Count <= 0)
            return 1;

        long data = 0;
        foreach(var move in moves)
        {
            doMoveTimer.Start();
            DoMove(move, board);
            doMoveTimer.Stop();

            if (move.ToUCI() == "d7c8r") 
            {
                foreach (var inMove in board.currentTurnMoves)
                    UnityEngine.Debug.Log("!"+inMove+" - "+move.piece.Board.Name);

                UnityEngine.Debug.Log("!!" + board.piecesHolder.blackQueens[0].PinnedBy);
            }

            long moveNodeCount = await Perft(depth - 1, board);
            data += moveNodeCount;


            undoMoveTimer.Start();
            UndoLastMove(board);
            undoMoveTimer.Stop();

            if(divide && currentDepth == depth) 
                divideDict.Add(move.ToUCI(), moveNodeCount);

            
        }

        return data;
    }

    protected virtual void DoMove(Move move, Board board) 
    {
        UnityEngine.Debug.Log(">>"+board.Name);
        manager.TurnManager.DoMove(move, board);
    }

    protected virtual void UndoLastMove(Board board)
    {
        manager.TurnManager.UndoLastMove(board);
    }
}
