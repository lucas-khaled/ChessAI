using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobilityHeuristic : Heuristic
{
    public MobilityHeuristic(GameManager manager, float weight) : base(manager, weight)
    {
    }

    public override float GetHeuristic(Board board)
    {
        var blackMovesQnt = 0;
        var whiteMovesQnt = 0;

        List<Piece> pieces = board.pieces;
        for (int i = 0; i< pieces.Count; i++)
        {
            var piece = pieces[i];
            Move[] moves = piece.pieceColor == board.ActualTurn ?
                manager.MoveChecker.GetMoves(piece) :
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
