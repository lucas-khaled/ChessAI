using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MinimaxAI : AIPlayer
{
    private int maxDepth = 2;
    private EndGameChecker endGameChecker;
    private PositionHeuristic heuristic;

    private const string HEURISTIC_DEBUG = "Heuristic";
    private const string MOVE_CHOICE_DEBUG = "Move Choice";
    private const string MOVE_MINIMAX_DEBUG = "Move Minimax";

    public MinimaxAI(GameManager manager, int depth = 2) : base(manager) 
    {
        maxDepth = depth;
        endGameChecker = new(null);
        heuristic = new SimplePositionHeuristic();
    } 

    protected override async Task<Move> CalculateMove()
    {
        var env = manager.environment;

        bool isWhite = actualColor == PieceColor.White;
        float bestScore = isWhite ? float.MinValue : float.MaxValue;
        List<Move> bestMoves = new List<Move>();
        Move bestMove = null;
        var alpha = float.MinValue;
        var beta = float.MaxValue;

        var moves = GetAllMoves(env, actualColor);

        Debug.Log($"Evaluating {moves.Count} moves");
        
        Stopwatch moveChoiceStopWatch = Stopwatch.StartNew();
        moveChoiceStopWatch.Start();

        foreach(var move in moves) 
        {
            Environment newEnv = env.Copy();
            newEnv.turnManager.DoMove(move);
            Debug.Log($"<color=cyan>{maxDepth} -> Evaluating {move}</color>");

            Stopwatch moveMinimaxStopWatch = Stopwatch.StartNew();
            moveMinimaxStopWatch.Start();

            float score = Minimax(newEnv, actualColor.GetOppositeColor(), maxDepth-1, alpha, beta);

            moveMinimaxStopWatch.Stop();
            Debugger.LogStopwatch(moveMinimaxStopWatch, MOVE_MINIMAX_DEBUG, true);

            Debug.Log($"<color=cyan>{maxDepth} -> Evaluated {move} \nwith a score of {score}</color>");

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


        Debug.Log($"<color=green>Choosed {bestMove} \nas best with a score of {bestScore}</color>");
        return bestMove;
    }

    private float Minimax(Environment env, PieceColor color, int depth, float alpha, float beta) 
    {
        bool isMaximize = color == PieceColor.White;
        endGameChecker.SetEnvironment(env);

        if (endGameChecker.IsCheckMate())
            return isMaximize ? 1000 - depth : -1000 + depth;

        if (endGameChecker.HasDraw())
            return 0;

        if (depth == 0) 
        {
            Stopwatch heuristicStopWatch = Stopwatch.StartNew();

            heuristicStopWatch.Start();
            float heuristicValue = heuristic.GetHeuristic(env);

            heuristicStopWatch.Stop();
            Debugger.LogStopwatch(heuristicStopWatch, HEURISTIC_DEBUG, true);

            Debug.Log($"Position Evaluation: {heuristicValue}");
            return heuristicValue;
        }

        float bestScore = isMaximize ? float.MinValue : float.MaxValue;
        foreach (var move in GetAllMoves(env, color)) 
        {
            Environment newEnv = env.Copy();
            newEnv.turnManager.DoMove(move);

            Debug.Log($"<color=yellow>{depth} -> Evaluating {move}</color>");

            float score = Minimax(newEnv, color.GetOppositeColor(), depth-1, alpha, beta);

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
