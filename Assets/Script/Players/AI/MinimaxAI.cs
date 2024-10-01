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

    public MinimaxAI(GameManager manager, int depth = 2) : base(manager) 
    {
        maxDepth = depth;
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

        var moves = SortMoves(GetAllMoves(board, actualColor));

        Debug.Log($"Evaluating {moves.Count} moves");
        
        Stopwatch moveChoiceStopWatch = Stopwatch.StartNew();
        moveChoiceStopWatch.Start();

        foreach(var move in moves) 
        {
            Stopwatch moveMinimaxStopWatch = Stopwatch.StartNew();
            moveMinimaxStopWatch.Start();

            manager.TurnManager.DoMove(move, board);

            float score = 0;

            if (transpositionTable.HasScore(board.ActualHash))
            {
                score = transpositionTable.GetScore(board.ActualHash);
                Debug.Log($"Got score {score} from table in position {board.ActualHash}");
            }
            else
            {
                score = Minimax(actualColor.GetOppositeColor(), maxDepth - 1, alpha, beta);
                transpositionTable.AddScore(board.ActualHash, score);
            }

            manager.TurnManager.UndoLastMove(board);

            moveMinimaxStopWatch.Stop();
            Debugger.LogStopwatch(moveMinimaxStopWatch, MOVE_MINIMAX_DEBUG, true);

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
            //Stopwatch heuristicStopWatch = Stopwatch.StartNew();

            //heuristicStopWatch.Start();
            float heuristicValue = heuristic.GetHeuristic(board);

            //heuristicStopWatch.Stop();
            //Debugger.LogStopwatch(heuristicStopWatch, HEURISTIC_DEBUG, true);

            evalCount++;
            return heuristicValue;
        }

        float bestScore = isMaximize ? float.MinValue : float.MaxValue;
        var moves = GetAllMoves(board, color);
        foreach (var move in moves) 
        {
            Stopwatch moveMinimaxStopWatch = Stopwatch.StartNew();
            moveMinimaxStopWatch.Start();

            manager.TurnManager.DoMove(move, board);

            float score = 0;

            if (transpositionTable.HasScore(board.ActualHash))
            {
                score = transpositionTable.GetScore(board.ActualHash);
                Debug.Log($"Got score {score} from table in position {board.ActualHash}");
            }
            else
            {
                score = Minimax(color.GetOppositeColor(), depth - 1, alpha, beta);
                transpositionTable.AddScore(board.ActualHash, score);
            }

            manager.TurnManager.UndoLastMove(board);

            moveMinimaxStopWatch.Stop();
            Debugger.LogStopwatch(moveMinimaxStopWatch, MOVE_MINIMAX_DEBUG + " - " + depth, true);


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
