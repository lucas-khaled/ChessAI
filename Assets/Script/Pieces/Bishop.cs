using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : BlockableMovesPiece
{
    public Bishop(Environment env) : base(env)
    {
    }

    public override Move[] GetMoves()
    {
        return GetDiagonalMoves().ToArray();
    }
}
