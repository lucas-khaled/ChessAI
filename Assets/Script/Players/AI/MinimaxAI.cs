using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MinimaxAI : AIPlayer
{
    private int maxDepth = 2;
    private EndGameChecker endGameChecker;

    public MinimaxAI(GameManager manager, int depth = 2) : base(manager) 
    {
        maxDepth = depth;
        endGameChecker = new(null);
    } 

    protected override async Task<Move> CalculateMove()
    {
        var env = manager.environment;

        bool isWhite = actualColor == PieceColor.White;
        float bestScore = isWhite ? float.MinValue : float.MaxValue;
        Move bestMove = null;

        var moves = GetAllMoves(env);

        Debug.Log($"Evaluating {moves.Count} moves");
        foreach(var move in moves) 
        {
            Environment newEnv = env.Copy();
            newEnv.turnManager.DoMove(move);
            float score = Minimax(newEnv, isWhite, maxDepth);

            Debug.Log($"Evaluated {move} \nwith a score of {score}");

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
            
        }

        Debug.Log($"Choosed {bestMove} \nas best with a score of {bestScore}");
        return bestMove;
    }


    private float Minimax(Environment env, bool isMaximize, int depth) 
    {
        endGameChecker.SetEnvironment(env);

        if (endGameChecker.IsCheckMate())
            return (isMaximize) ? 1000 - depth : -1000 + depth;

        if (endGameChecker.HasDraw())
            return 0;

        if (depth == 0)
            return GetHeuristicOnPosition(env);

        float bestScore = isMaximize ? float.MinValue : float.MaxValue;
        foreach (var move in GetAllMoves(env)) 
        {
            Environment newEnv = env.Copy();
            newEnv.turnManager.DoMove(move);

            float score = Minimax(newEnv, !isMaximize, depth-1);

            bestScore = isMaximize ? Mathf.Max(bestScore, score) : Mathf.Min(bestScore, score);
        }

        return bestScore;
    }

    private bool IsBetterScoreThan(float newScore, float oldScore)
    {
        return (actualColor == PieceColor.White) ? newScore > oldScore : newScore < oldScore;
    }

    private float GetHeuristicOnPosition(Environment environment)
    {
        return GetHeuristicOnMaterial(environment);
    }

    private float GetHeuristicOnMaterial(Environment environment)
    {
        var pieces = environment.board.pieces;

        return QueenMaterialCount(pieces) + RookMaterialCount(pieces) + BishopMaterialCount(pieces) + KnightMaterialCount(pieces) + PawnMaterialCount(pieces);
    }

    private float QueenMaterialCount(List<Piece> pieces) 
    {
        var score = GetMaterialCount(pieces.Where(x => x is Queen).ToList(), 10);
        return score;
    }

    private float RookMaterialCount(List<Piece> pieces) 
    {
        var score = GetMaterialCount(pieces.Where(x => x is Rook).ToList(), 5);
        return score;
    }

    private float BishopMaterialCount(List<Piece> pieces)
    {
        var score = GetMaterialCount(pieces.Where(x => x is Bishop).ToList(), 3);
        return score;
    }

    private float KnightMaterialCount(List<Piece> pieces)
    {
        var score = GetMaterialCount(pieces.Where(x => x is Knight).ToList(), 3);
        return score;
    }
    private float PawnMaterialCount(List<Piece> pieces)
    {
        var score = GetMaterialCount(pieces.Where(x => x is Pawn).ToList(), 1);
        return score;
    }

    private float GetMaterialCount(List<Piece> pieces, float weight) 
    {
        int wQnt = pieces.Count(x => x.pieceColor == PieceColor.White);
        int bQnt = pieces.Count(x => x.pieceColor == PieceColor.Black);

        return weight * (wQnt - bQnt);
    }
}
