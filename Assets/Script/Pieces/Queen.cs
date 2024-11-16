using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : SlidingPieces
{
    public Queen(Board board) : base(board)
    {
    }

    public override Move[] GetMoves()
    {
        List<Move> moves = new();

        moves.AddRange(GetDiagonalMoves());
        moves.AddRange(GetVerticalMoves());
        moves.AddRange(GetHorizontalMoves());

        return moves.ToArray();
    }
}
