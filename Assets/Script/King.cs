using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : BlockableMovesPiece
{
    public override Move[] GetMoves(Board board)
    {
        List<Move> moves = new List<Move>();

        var horizontal = GetHorizontalMoves(board, 1);
        var vertical = GetVerticalMoves(board, 1);
        var diagonals = GetDiagonalMoves(board, 1);

        moves.AddRange(horizontal);
        moves.AddRange(vertical);
        moves.AddRange(diagonals);

        return moves.ToArray();
    }
}
