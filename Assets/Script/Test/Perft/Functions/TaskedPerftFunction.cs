using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public class TaskedPerftFunction : PerftFunction
{
    protected int currentDepth;
    protected bool isPerfting;
    protected bool divide;
    protected bool debugAll;

    private Stopwatch doMoveTimer = new();
    private Stopwatch undoMoveTimer = new();
    private Stopwatch perftTimer = new();

    public override async Task<PerftData> Perft(int depth, bool divide = true, bool debugAll = false) 
    {
        if (isPerfting)
        {
            return PerftData.NotValid;
        }

        perftTimer.Reset();
        doMoveTimer.Reset();
        undoMoveTimer.Reset();

        this.divide = divide;
        this.debugAll = debugAll;

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

        //long medianDoTime = doMoveTimer.ElapsedMilliseconds / result.nodes;
        //long medianUndoTime = undoMoveTimer.ElapsedMilliseconds / result.nodes;

        UnityEngine.Debug.Log($"Finished Perft with depth {currentDepth}:\n " +
            $"Perft time: {perftTimer.ElapsedMilliseconds}ms\n"); //+
            //$"Do time: {medianDoTime}ms\n" +
            //$"Undo time: {medianUndoTime}ms\n");

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
            //doMoveTimer.Start();

            if(move.to.TilePosition.ToString() == "c6" && move.piece is Pawn pawn 
                && pawn.pieceColor == PieceColor.White && board.rules.enPassantTileCoordinates.ToString() == "c6") 
            {
                string sequence = string.Empty;
                for (int i = 0; i < board.turns.Count; i++)
                {
                    var turn = board.turns[i];
                    if (turn.move == null) continue;

                    sequence += turn.move.ToUCI() + " - ";
                }

                string enPassantString = board.rules.HasEnPassant ? board.rules.enPassantTileCoordinates.ToString() : "-";
                UnityEngine.Debug.Log($"Moving pawn to c5 in depth {depth}" +
                    $"\nSequence: {sequence}" +
                    $"\nEn Passant: {board.rules.HasEnPassant} {enPassantString}" +
                    $"\nBoard representation {board.moveGenerator.GetActualBoardBitboard()}" +
                    $"\n{move}");
            }
            
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

                    sequence += turn.move.ToUCI() + " - ";
                }

                UnityEngine.Debug.LogError($"Erro while checking in depth {depth} move {move.ToUCI()}" +
                    $"\nPiece tile {move.piece.GetTile().TilePosition}" +
                    $"\nPiece Color is {move.piece.pieceColor}" +
                    $"\n Move Sequence: {sequence}\n {e}");
                continue;
            }
            //doMoveTimer.Stop();

            PerftData moveNodeCount = await Perft(depth - 1, board);
            data += moveNodeCount;

            //undoMoveTimer.Start();
            UndoLastMove(board);
           // undoMoveTimer.Stop();

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
