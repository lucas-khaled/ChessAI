using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

public class TaskedPerftFunction : PerftFunction
{
    protected int currentDepth;
    protected bool isPerfting;
    protected bool divide;
    protected bool debugAll;

    private int iterationNum = 0;

    public override async Task<PerftData> Perft(int depth, bool divide = true, bool debugAll = false) 
    {
        if (isPerfting)
        {
            return PerftData.NotValid;
        }

        this.divide = divide;
        this.debugAll = debugAll;

        isPerfting = true;
        currentDepth = depth;
        iterationNum = 0;

        var data = await Task.Run(PerftTask);

        isPerfting = false;

        return data;
    }

    protected virtual async Task<PerftData> PerftTask() 
    {
        UnityEngine.Debug.Log($"Starting Perft with depth {currentDepth}");

        PerftData result = await Perft(currentDepth, manager.TestBoard);

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
        iterationNum++;
        if(debugAll)
            UnityEngine.Debug.Log("Perfting: "+iterationNum);

        var moves = new List<Move>(board.currentTurnMoves);

        if (debugAll is false)
        {
            if (depth == 1)
            {
                PerftData returnData = new PerftData(moves.Count);
                if (depth == currentDepth)
                {
                    foreach (Move move in moves)
                        returnData.divideDict.Add(new PerftDivide(move.ToUCI(), 1));
                }

                return returnData;
            }
        }
        else 
        {
            if(depth == 0) 
            {
                PerftData returnData = PerftData.Single;

                if (board.IsCheckMate)
                {
                    returnData.checkmates = 1;
                    return returnData;
                }

                if (board.moveGenerator.IsCheck())
                    returnData.checks = 1;

                if (board.moveGenerator.IsDoubleCheck())
                    returnData.doubleChecks = 1;

                returnData.captures = board.LastTurn.move.capture != null ? 1 : 0;
                returnData.enPassants = board.LastTurn.move.capture != null
                    && board.LastTurn.move.to.TilePosition.Equals(board.LastTurn.enPassant)
                    ? 1 : 0;

                returnData.castles = board.LastTurn.move is CastleMove ? 1 : 0;
                returnData.promotions = board.LastTurn.move is PromotionMove ? 1 : 0;
                   

                return returnData;
            }
        }

        if (moves == null || moves.Count <= 0)
            return PerftData.Single;

        PerftData data = PerftData.Empty;
        foreach(var move in moves)
        {
            try
            {
                DoMove(move, board);
            }
            catch (Exception e) 
            {
                string sequence = string.Empty;
                for (int i = 0; i < board.turns.Count; i++) 
                {
                    var turn = board.turns[i];
                    if (turn.move == null) continue;

                    sequence += turn.move.ToUCI() + " - " + turn.move.piece;
                }

                UnityEngine.Debug.LogError($"Erro while checking in depth {depth} move {move.ToUCI()} on iteration {iterationNum}" +
                    $"\nPiece tile {move.piece.GetTile().TilePosition}" +
                    $"\nPiece Color is {move.piece.pieceColor}" +
                    $"\n Move Sequence: {sequence}\n {e}");
                continue;
            }

            PerftData moveNodeCount = await Perft(depth - 1, board);
            data += moveNodeCount;

            UndoLastMove(board);

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
