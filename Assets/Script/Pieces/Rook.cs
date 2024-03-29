using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : BlockableMovesPiece
{
    public Rook(Environment env) : base(env)
    {
    }

    public override Move[] GetMoves()
    {
        List<Move> moves = new();
        moves.AddRange(GetVerticalMoves());
        moves.AddRange(GetHorizontalMoves());

        return moves.ToArray();
    }
}
