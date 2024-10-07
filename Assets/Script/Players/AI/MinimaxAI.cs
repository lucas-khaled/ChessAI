using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MinimaxAI : AIPlayer
{
    private int maxDepth = 2;
    private PositionHeuristic heuristic;
    private TranspositionTable transpositionTable;

    private const string HEURISTIC_DEBUG = "Heuristic";
    private const string MOVE_CHOICE_DEBUG = "Move Choice";
    private const string MOVE_MINIMAX_DEBUG = "Move Minimax";

    private int evalCount = 0;
    private int timeLimit = -1;

    public MinimaxAI(GameManager manager, int depth = 2, int timeLimit = 60000) : base(manager) 
    {
        maxDepth = depth;
        this.timeLimit = timeLimit;
        heuristic = new SimplePositionHeuristic(manager);
        transpositionTable = new TranspositionTable();
    } 

    protected override async Task<Move> CalculateMove()
    {
        var board = manager.TestBoard;

        bool isWhite = actualColor == PieceColor.White;
        float bestScore = isWhite ? float.MinValue : float.MaxValue;
        List<Move> bestMoves = new List<Move>();
        Move bestMove = null;
        var alpha = float.MinValue;
        var beta = float.MaxValue;
        evalCount = 0;

        var unsortedMoves = GetAllMoves(board, actualColor);
        var moves = SortMoves(unsortedMoves);

        Debug.Log($"Evaluating {moves.Count} moves");
        
        Stopwatch moveChoiceStopWatch = Stopwatch.StartNew();
        moveChoiceStopWatch.Start();

        Stopwatch timeLimitChecker = Stopwatch.StartNew();
        timeLimitChecker.Start();
        for (int depth = 1; depth < maxDepth; depth++)
        {
            foreach (var move in moves)
            {
                if (timeLimit > 0 && timeLimitChecker.ElapsedMilliseconds >= timeLimit)
                    break;

                manager.TurnManager.DoMove(move, board);

                float score = 0;

                if (transpositionTable.HasScore(board.ActualHash))
                    score = transpositionTable.GetScore(board.ActualHash);
                else
                {
                    if (manager.EndGameChecker.IsCheckMate(board))
                    {
                        bestMoves.Clear();
                        bestMoves.Add(move);

                        score = (actualColor == PieceColor.White) ? 1000 : -1000;
                        transpositionTable.AddScore(board.ActualHash, score);

                        manager.TurnManager.UndoLastMove(board);
                        break;
                    }

                    score = Minimax(actualColor.GetOppositeColor(), depth, alpha, beta);
                    transpositionTable.AddScore(board.ActualHash, score);

                }

                manager.TurnManager.UndoLastMove(board);

                if (IsBetterScoreThan(score, bestScore))
                {
                    bestMoves.Clear();
                    bestMoves.Add(move);
                    bestScore = score;
                }
                else if (score == bestScore)
                {
                    bestMoves.Add(move);
                }

                if (isWhite)
                    alpha = Mathf.Max(alpha, score);
                else
                    beta = Mathf.Min(beta, score);

                if (beta <= alpha) break;
            }
        }

        timeLimitChecker.Stop();

        var rand = new System.Random();

        int choice = rand.Next(0, bestMoves.Count);
        bestMove = bestMoves[choice];

        moveChoiceStopWatch.Stop();
        Debugger.LogStopwatch(moveChoiceStopWatch, MOVE_CHOICE_DEBUG, true);
        Debugger.LogTimeRecord(MOVE_CHOICE_DEBUG, "Move Choice final record");

        Debug.Log($"Evaluated {evalCount} times");

        Debug.Log($"<color=green>Choosed {bestMove} \nas best with a score of {bestScore}</color>");
        return bestMove;
    }

    private float Minimax(PieceColor color, int depth, float alpha, float beta) 
    {
        var board = manager.TestBoard;
        bool isMaximize = color == PieceColor.White;

        if (manager.EndGameChecker.HasDraw(board))
            return 0;

        if (manager.EndGameChecker.IsCheckMate(board))
            return !isMaximize ? 1000 - (maxDepth-depth): -1000 + (maxDepth-depth);

        if (depth == 0) 
        {
            float heuristicValue = heuristic.GetHeuristic(board);

            evalCount++;
            return heuristicValue;
        }

        float bestScore = isMaximize ? float.MinValue : float.MaxValue;
        var moves = SortMoves(GetAllMoves(board, color));
        foreach (var move in moves) 
        {
            manager.TurnManager.DoMove(move, board);

            float score = 0;

            if (transpositionTable.HasScore(board.ActualHash))
                score = transpositionTable.GetScore(board.ActualHash);
            else
            {
                score = Minimax(color.GetOppositeColor(), depth - 1, alpha, beta);
                transpositionTable.AddScore(board.ActualHash, score);
            }

            manager.TurnManager.UndoLastMove(board);

            if (isMaximize)
            {
                bestScore = Mathf.Max(bestScore, score);
                alpha = Mathf.Max(alpha, score);
            }
            else 
            {
                bestScore = Mathf.Min(bestScore, score);
                beta = Mathf.Min(beta, score);
            }

            if (beta <= alpha)
                break;
        }

        return bestScore;
    }

    private bool IsBetterScoreThan(float newScore, float oldScore)
    {
        return (actualColor == PieceColor.White) ? newScore > oldScore : newScore < oldScore;
    }
}
