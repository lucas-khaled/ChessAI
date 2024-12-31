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

    private Stopwatch doMoveTimer = new();
    private Stopwatch undoMoveTimer = new();
    private Stopwatch perftTimer = new();

    public override async Task<PerftData> Perft(int depth, bool divide = true) 
    {
        if (isPerfting)
        {
            return PerftData.NotValid;
        }

        perftTimer.Reset();
        doMoveTimer.Reset();
        undoMoveTimer.Reset();

        this.divide = divide;

        isPerfting = true;
        currentDepth = depth;

        var data = await Task.Run(PerftTask);

        isPerfting = false;

        return data;
    }

    protected virtual async Task<PerftData> PerftTask() 
    {
        UnityEngine.Debug.Log($"Starting Perft with depth {currentDepth}");

        perftTimer.Start();
        PerftData result = await Perft(currentDepth, manager.TestBoard);
        perftTimer.Stop();

        long medianDoTime = doMoveTimer.ElapsedMilliseconds / result.nodes;
        long medianUndoTime = undoMoveTimer.ElapsedMilliseconds / result.nodes;

        UnityEngine.Debug.Log($"Finished Perft with depth {currentDepth}:\n " +
            $"Perft time: {perftTimer.ElapsedMilliseconds}ms\n" +
            $"Do time: {medianDoTime}ms\n" +
            $"Undo time: {medianUndoTime}ms\n");

        if (divide)
            DebugDivide(result);

        return result;
    }

    private void DebugDivide(PerftData result)
    {
        string debugString = string.Empty;
        foreach(var divide in result.divideDict) 
        {
            debugString += $"{divide.move}: {divide.nodeCount}\n";
        }

        UnityEngine.Debug.Log("Divide:\n"+debugString);
    }

    protected virtual async Task<PerftData> Perft(int depth, Board board) 
    {
        var moves = new List<Move>(board.currentTurnMoves);

        if (depth == 1)
        {
            PerftData returnData = new PerftData(moves.Count);
            if (depth == currentDepth)
            {
                foreach(Move move in moves)
                    returnData.divideDict.Add(new PerftDivide(move.ToUCI(), 1));
            }

            return returnData;
        }

        if (moves == null || moves.Count <= 0)
            return PerftData.Single;

        PerftData data = PerftData.Empty;
        foreach(var move in moves)
        {
            doMoveTimer.Start();
            DoMove(move, board);
            doMoveTimer.Stop();

            /*if (move.ToUCI() == "d7c8r" || move.ToUCI() == "d7c8q") 
            {
                foreach (var inMove in board.currentTurnMoves)
                {
                    if (inMove.piece is Queen queen)
                    {
                        PinnerPiece pinner = (PinnerPiece)board.GetTileByIndex(58).OccupiedBy;
                        UnityEngine.Debug.Log("! " + move.ToUCI() 
                            + "\nBitboard: " + pinner.PinSquares.ToVisualString()
                            + "\n" + inMove 
                            + "\nMove piece Board: " + move.piece.Board.Name 
                            + "\n InMove piece Board: " + inMove.piece.Board.Name
                            + "\n Pinning: " + pinner.PinningIndex
                            + "\n Hashs: " + pinner.PinningIndex.GetHashCode() + " - " + queen.GetHashCode()
                            + "\nPinnedBy: " + queen.PinnedBy);
                    }
                }
            }*/

            PerftData moveNodeCount = await Perft(depth - 1, board);
            data += moveNodeCount;

            undoMoveTimer.Start();
            UndoLastMove(board);
            undoMoveTimer.Stop();

            if(divide && currentDepth == depth) 
                data.divideDict.Add(new PerftDivide(move.ToUCI(), moveNodeCount.nodes));

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
