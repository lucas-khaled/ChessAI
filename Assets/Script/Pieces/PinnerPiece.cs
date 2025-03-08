using System.Collections.Generic;
using UnityEngine;

public abstract class PinnerPiece : SlidingPieces
{
    public Bitboard KingDangerSquares { get; protected set; } = new Bitboard();
    public Bitboard InBetweenSquares { get; protected set; } = new Bitboard();

    protected PinnerPiece(Board board) : base(board)
    {
    }

    public override void GenerateBitBoard()
    {
        KingDangerSquares.Clear();
        InBetweenSquares.Clear();
        base.GenerateBitBoard();
    }

    protected void GeneratePinAndKingDangerBySegment(List<TileCoordinates> segment)
    {
        Bitboard kingDanger = new();
        Bitboard inBetween = new(actualTile.Bitboard);

        bool hasKing = false;
        bool hasSomethingBetween = false;
        bool hasSomethingElseThenKingInSegment = false;

        foreach (var tileCoord in segment)
        {
            Tile tile = Board.tiles[tileCoord.row][tileCoord.column];

            if(hasSomethingElseThenKingInSegment is false)
                kingDanger.Add(tile.Bitboard);

            if (tile.IsOccupied)
            {
                if(tile.OccupiedBy.IsEnemyPiece(this) is false || tile.OccupiedBy is not King) 
                {
                    if(hasKing is false)
                        hasSomethingBetween = true;

                    hasSomethingElseThenKingInSegment = true;
                }
                else
                {
                    hasKing = true;
                    continue;
                }
            }

            if (hasKing is false)
                inBetween.Add(tile.Bitboard);

            if (hasKing && hasSomethingBetween && hasSomethingElseThenKingInSegment) break;
        }

        if (hasKing) 
        {
            InBetweenSquares = inBetween;

            if(hasSomethingBetween is false)
                KingDangerSquares.Add(kingDanger);
        }
    }
}
