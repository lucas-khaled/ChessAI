using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : SlidingPieces
{
    public Bishop(Board board) : base(board)
    {
    }

    public override Move[] GetMoves()
    {
        return GetDiagonalMoves().ToArray();
    }
}
