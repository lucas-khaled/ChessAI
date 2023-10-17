using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : BlockableMovesPiece
{
    public override Move[] GetMoves(Board board)
    {
        List<Move> moves = new();

        moves.AddRange(GetDiagonalMoves(board));
        moves.AddRange(GetVerticalMoves(board));
        moves.AddRange(GetHorizontalMoves(board));

        return moves.ToArray();
    }
}
