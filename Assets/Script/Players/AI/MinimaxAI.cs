using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MinimaxAI : AIPlayer
{
    private int maxDepth = 2;
    private PositionHeuristic heuristic;

    private const string HEURISTIC_DEBUG = "Heuristic";
    private const string MOVE_CHOICE_DEBUG = "Move Choice";
    private const string MOVE_MINIMAX_DEBUG = "Move Minimax";

    private int evalCount = 0;

    public MinimaxAI(GameManager manager, int depth = 2) : base(manager) 
    {
        maxDepth = depth;
        heuristic = new SimplePositionHeuristic(manager);
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

        var moves = GetAllMoves(board, actualColor);

        Debug.Log($"Evaluating {moves.Count} moves");
        
        Stopwatch moveChoiceStopWatch = Stopwatch.StartNew();
        moveChoiceStopWatch.Start();

        foreach(var move in moves) 
        {
            manager.TurnManager.DoMove(move, board);
            Debug.Log($"<color=cyan>{maxDepth} -> Evaluating {move}</color>");

            Stopwatch moveMinimaxStopWatch = Stopwatch.StartNew();
            moveMinimaxStopWatch.Start();

            float score = Minimax(actualColor.GetOppositeColor(), maxDepth-1, alpha, beta);

            moveMinimaxStopWatch.Stop();
            Debugger.LogStopwatch(moveMinimaxStopWatch, MOVE_MINIMAX_DEBUG, true);

            Debug.Log($"<color=cyan>{maxDepth} -> Evaluated {move} \nwith a score of {score}</color>");

            manager.TurnManager.UndoLastMove(board);

            if (IsBetterScoreThan(score, bestScore))
            {
                bestMoves.Clear();
                bestMoves.Add(move);
                bestScore = score;
            }
            else if(score == bestScore) 
            {
                bestMoves.Add(move);
            }

            if (isWhite)
                alpha = Mathf.Max(alpha, score);
            else
                beta = Mathf.Min(beta, score);

            if (beta <= alpha) break;
        }

        Debugger.LogTimeRecord(HEURISTIC_DEBUG, "Heuristic final record");
        Debugger.LogTimeRecord(MOVE_MINIMAX_DEBUG, "Move Minimax final record");

        bestMoves.ForEach(x => Debug.Log($"<color=green>One of best was: {x}</color>"));

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

        if (manager.EndGameChecker.IsCheckMate(board))
            return isMaximize ? 1000 - depth : -1000 + depth;

        if (manager.EndGameChecker.HasDraw(board))
            return 0;

        if (depth == 0) 
        {
            Stopwatch heuristicStopWatch = Stopwatch.StartNew();

            heuristicStopWatch.Start();
            float heuristicValue = heuristic.GetHeuristic(board);

            heuristicStopWatch.Stop();
            Debugger.LogStopwatch(heuristicStopWatch, HEURISTIC_DEBUG, true);

            Debug.Log($"Position Evaluation: {heuristicValue}");
            evalCount++;
            return heuristicValue;
        }

        float bestScore = isMaximize ? float.MinValue : float.MaxValue;
        foreach (var move in GetAllMoves(board, color)) 
        {
            manager.TurnManager.DoMove(move, board);

            Debug.Log($"<color=yellow>{depth} -> Evaluating {move}</color>");

            float score = Minimax(color.GetOppositeColor(), depth-1, alpha, beta);

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
            {
                Debug.Log($"<color=red>{depth} -> Prunned with alpha = {alpha} and beta = {beta}</color>");
                break;
            }
        }

        return bestScore;
    }

    private bool IsBetterScoreThan(float newScore, float oldScore)
    {
        return (actualColor == PieceColor.White) ? newScore > oldScore : newScore < oldScore;
    }
}
