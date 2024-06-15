using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MinimaxAI : AIPlayer
{
    private int maxDepth = 2;
    private EndGameChecker endGameChecker;
    private PositionHeuristic heuristic;

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
        Move bestMove = null;
        var alpha = float.MinValue;
        var beta = float.MaxValue;

        var moves = GetAllMoves(env, actualColor);

        Debug.Log($"Evaluating {moves.Count} moves");
        foreach(var move in moves) 
        {
            Environment newEnv = env.Copy();
            newEnv.turnManager.DoMove(move);
            float score = Minimax(newEnv, actualColor.GetOppositeColor(), maxDepth-1, alpha, beta);

            Debug.Log($"<color=blue>{maxDepth} -> Evaluated {move} \nwith a score of {score}</color>");

            if (IsBetterScoreThan(score, bestScore))
            {
                bestMove = move;
                bestScore = score;
            }
            else if(score == bestScore) 
            {
                var rand = new System.Random();
                var chanceToChange = rand.Next(1, 100);

                if(chanceToChange > 50) 
                    bestMove = move;
            }

            if (isWhite)
                alpha = Mathf.Max(alpha, score);
            else
                beta = Mathf.Min(beta, score);

            if (beta <= alpha) break;
        }

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
            return heuristic.GetHeuristic(env);

        float bestScore = isMaximize ? float.MinValue : float.MaxValue;
        foreach (var move in GetAllMoves(env, color)) 
        {
            Environment newEnv = env.Copy();
            newEnv.turnManager.DoMove(move);

            float score = Minimax(newEnv, color.GetOppositeColor(), depth-1, alpha, beta);

            Debug.Log($"<color=yellow>{depth} -> Evaluated {move} \nwith a score of {score}</color>");
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
