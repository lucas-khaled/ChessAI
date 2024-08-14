using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobilityHeuristic : Heuristic
{
    public MobilityHeuristic(float weight) : base(weight)
    {
    }

    public override float GetHeuristic(Environment environment)
    {
        FENManager fenManager = new FENManager(environment);
        var blackMovesQnt = 0;
        var whiteMovesQnt = 0;

        foreach (var piece in environment.board.pieces)
        {
            Move[] moves = piece.pieceColor == environment.turnManager.ActualTurn ?
                environment.moveMaker.GetMoves(piece) :
                piece.GetMoves();

            if (moves.Length <= 0) continue;

            if (piece.pieceColor == PieceColor.White)
                whiteMovesQnt += moves.Length;
            else
                blackMovesQnt += moves.Length;
        }

        float finalHeuristic = weight * (whiteMovesQnt - blackMovesQnt);
        return finalHeuristic;
    }
}
