using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : BlockableMovesPiece
{
    public King(Environment env) : base(env)
    {
    }

    public override Move[] GetMoves()
    {
        List<Move> moves = new List<Move>();

        var horizontal = GetHorizontalMoves(1);
        var vertical = GetVerticalMoves(1);
        var diagonals = GetDiagonalMoves(1);

        moves.AddRange(horizontal);
        moves.AddRange(vertical);
        moves.AddRange(diagonals);

        return moves.ToArray();
    }
}
