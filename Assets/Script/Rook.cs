using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : BlockableMovesPiece
{
    public override Move[] GetMoves(Board board)
    {
        List<Move> moves = new();
        moves.AddRange(GetVerticalMoves(board));
        moves.AddRange(GetHorizontalMoves(board));

        return moves.ToArray();
    }
}
