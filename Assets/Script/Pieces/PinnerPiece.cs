using System.Collections.Generic;
using UnityEngine;

public abstract class PinnerPiece : SlidingPieces
{
    public Bitboard KingDangerSquares { get; protected set; } = new Bitboard();
    public Bitboard PinSquares { get; protected set; } = new Bitboard();
    public Bitboard InBetweenSquares { get; protected set; } = new Bitboard();
    public int PinningIndex { get; protected set; }

    protected PinnerPiece(Board board) : base(board)
    {
    }

    public override void GenerateBitBoard()
    {
        PinningIndex = -1;
        KingDangerSquares.Clear();
        PinSquares.Clear();
        InBetweenSquares.Clear();
        base.GenerateBitBoard();
    }

    protected void GeneratePinAndKingDangerBySegment(List<TileCoordinates> segment)
    {
        Bitboard kingDanger = new();
        Bitboard pinSquares = new();
        Bitboard inBetween = new();

        int friendlyPieces = 0;
        bool hasKing = false;
        bool hasPin = false;
        int enemieInBetweenIndex = -1;

        foreach (var tileCoord in segment)
        {
            Tile tile = Board.tiles[tileCoord.row][tileCoord.column];

            if(hasKing is false)
                inBetween.Add(tile.Bitboard);

            if (tile.IsOccupied)
            {
                if (tile.OccupiedBy.pieceColor == pieceColor) 
                {
                    if (hasKing)
                    {
                        if (friendlyPieces <= 0)
                            kingDanger.Add(tile.Bitboard);

                        inBetween.Remove(tile.Bitboard);
                        break;
                    }


                    friendlyPieces++;
                    continue;
                }

                if (tile.OccupiedBy is King)
                {
                    hasKing = true;
                    hasPin = (enemieInBetweenIndex > -1) && friendlyPieces <= 0;
                    inBetween.Remove(tile.Bitboard);

                    if (hasPin || friendlyPieces > 0)
                        break;

                    kingDanger.Add(tile.Bitboard);
                    continue;
                }

                if (friendlyPieces > 0) continue;

                if (enemieInBetweenIndex > -1)
                {
                    hasPin = false;
                    break;
                }

                enemieInBetweenIndex = tile.Index;
                kingDanger.Add(tile.Bitboard);
                pinSquares.Add(tile.Bitboard);
                continue;
            }

            if(enemieInBetweenIndex == -1 && friendlyPieces <= 0)
                kingDanger.Add(tile.Bitboard);

            if (hasKing is false)
                pinSquares.Add(tile.Bitboard);
        }

        if (hasKing) 
        {
            InBetweenSquares = inBetween;
            KingDangerSquares.Add(kingDanger);
            if (hasPin)
            {
                PinningIndex = enemieInBetweenIndex;
                PinSquares.Add(pinSquares);
            }
        }
    }
}
