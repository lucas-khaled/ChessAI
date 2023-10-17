using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : BlockableMovesPiece
{
    public override Move[] GetMoves(Board board)
    {
        return GetDiagonalMoves(board).ToArray();
    }
}
