using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rook : SlidingPieces
{
    public Rook(Board board) : base(board)
    {
    }

    public override void GenerateBitBoard()
    {
        List<Tile> attackingTiles = new List<Tile>();

        attackingTiles.AddRange(GetVerticalBlockedSquares());
        attackingTiles.AddRange(GetHorizontalBlockedSquares());

        AttackingSquares = AddTilesBitBoards(attackingTiles);
    }

    public override Move[] GetMoves()
    {
        List<Move> moves = new();
        moves.AddRange(GetVerticalMoves());
        moves.AddRange(GetHorizontalMoves());

        return moves.ToArray();
    }
}
