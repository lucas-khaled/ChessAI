using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class MinimaxAI : AIPlayer
{
    private uint maxDepth = 2;
    private EndGameChecker endGameChecker;

    public MinimaxAI(GameManager manager, uint depth = 2) : base(manager) 
    {
        maxDepth = depth;
        endGameChecker = new(null);
    } 
    protected override async Task<Move> CalculateMove()
    {
        return GetBestMoveFrom(GetAllMoves(manager.environment), actualColor);
    }

    private Move GetBestMoveFrom(List<Move> moves, PieceColor actualColor) 
    {
        float bestScore = (actualColor == PieceColor.White) ? float.MinValue : float.MaxValue;
        Move bestMove = null;
        foreach (var move in moves)
        {
            float score = Evaluate(manager.environment, move);

            Debug.Log($"Score of {score} for {move}");
            if (IsBetterScoreThan(score, bestScore))
            {
                bestScore = score;
                bestMove = move;
            }
        }

        Debug.Log($"Choosed score {bestScore} for move {bestMove}");
        return bestMove;
    }

    private float Evaluate(Environment environment, Move move, int depth = 1) 
    {
        Environment env = environment.Copy();
        endGameChecker.SetEnvironment(env);
        
        PieceColor moveColor = move.piece.pieceColor;
        env.turnManager.DoMove(move);

        if (endGameChecker.IsCheckMate()) 
            return (moveColor == actualColor) ? 1000 - depth : -1000 + depth;
        
        if (endGameChecker.HasDraw()) 
            return 0;

        if (depth < maxDepth)
            return Evaluate(env, GetBestMoveFrom(GetAllMoves(env), env.turnManager.ActualTurn), depth+1);

        return GetHeuristicOnPosition(env);
    }

    private bool IsBetterScoreThan(float newScore, float oldScore) 
    {
        return (actualColor == PieceColor.White) ? newScore > oldScore : newScore < oldScore;
    }

    private float GetHeuristicOnPosition(Environment environment)
    {
        return GetEuristicOnMaterial(environment);
    }

    private float GetEuristicOnMaterial(Environment environment)
    {
        var pieces = environment.board.pieces;

        return QueenMaterialCount(pieces) + RookMaterialCount(pieces) + BishopMaterialCount(pieces) + KnightMaterialCount(pieces) + PawnMaterialCount(pieces);
    }

    private float QueenMaterialCount(List<Piece> pieces) 
    {
        var score = GetMaterialCount(pieces.Where(x => x is Queen).ToList(), 10);
        Debug.Log($"Queen score {score}");
        return score;
    }

    private float RookMaterialCount(List<Piece> pieces) 
    {
        var score = GetMaterialCount(pieces.Where(x => x is Rook).ToList(), 5);
        Debug.Log($"Rook score {score}");
        return score;
    }

    private float BishopMaterialCount(List<Piece> pieces)
    {
        var score = GetMaterialCount(pieces.Where(x => x is Bishop).ToList(), 3);
        Debug.Log($"Bishop score {score}");
        return score;
    }

    private float KnightMaterialCount(List<Piece> pieces)
    {
        var score = GetMaterialCount(pieces.Where(x => x is Knight).ToList(), 3);
        Debug.Log($"Knight score {score}");
        return score;
    }
    private float PawnMaterialCount(List<Piece> pieces)
    {
        var score = GetMaterialCount(pieces.Where(x => x is Pawn).ToList(), 1);
        Debug.Log($"Pawn score {score}");
        return score;
    }

    private float GetMaterialCount(List<Piece> pieces, float weight) 
    {
        int wQnt = pieces.Count(x => x.pieceColor == PieceColor.White);
        int bQnt = pieces.Count(x => x.pieceColor == PieceColor.Black);

        Debug.Log($"White Qnt {wQnt} and Black Qnt {bQnt}");
        return weight * (wQnt - bQnt);
    }
}
