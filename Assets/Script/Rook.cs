using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : BlockableMovesPiece
{
    public override Move[] GetPossibleMoves()
    {
        List<Move> moves = new();
        moves.AddRange(GetVerticalMoves());
        moves.AddRange(GetHorizontalMoves());

        return moves.ToArray();
    }
}
