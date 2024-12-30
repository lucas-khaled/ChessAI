using System.Collections.Generic;
using UnityEngine;

public abstract class PinnerPiece : SlidingPieces
{
    public Bitboard KingDangerSquares { get; protected set; } = new Bitboard();
    public Bitboard PinSquares { get; protected set; } = new Bitboard();
    public Piece Pinning { get; protected set; }

    protected PinnerPiece(Board board) : base(board)
    {
    }

    public override void GenerateBitBoard()
    {
        Pinning = null;
        KingDangerSquares.Clear();
        base.GenerateBitBoard();
    }

    protected void GeneratePinAndKingDangerBySegment(List<TileCoordinates> segment)
    {
        Bitboard kingDanger = new();
        Bitboard pinSquares = new();
        bool hasKing = false;
        bool hasPin = false;
        Piece enemieInBetween = null;

        foreach (var tileCoord in segment)
        {
            Tile tile = Board.tiles[tileCoord.row][tileCoord.column];

            if (tile.IsOccupied)
            {
                if (tile.OccupiedBy.pieceColor == pieceColor)
                    break;

                if (tile.OccupiedBy is King)
                {
                    hasKing = true;
                    hasPin = enemieInBetween != null;

                    if (hasPin)
                        break;

                    kingDanger.Add(tile.Bitboard);
                    continue;
                }

                if (enemieInBetween != null)
                {
                    hasPin = false;
                    break;
                }

                enemieInBetween = tile.OccupiedBy;
                kingDanger.Add(tile.Bitboard);
                pinSquares.Add(tile.Bitboard);
                continue;
            }

            if(enemieInBetween == null)
                kingDanger.Add(tile.Bitboard);

            if (hasKing is false)
                pinSquares.Add(tile.Bitboard);
        }

        /*if(this is Rook && pieceColor == PieceColor.White && Coordinates.row == 7 && segment.Count > 0 && segment[0].column == 3) 
        {
            Debug.Log($"!!!{hasKing} - {enemieInBetween} - {Board.Name} - {enemieInBetween.Board.Name}");
            foreach(var turn in Board.turns) 
            {
                Debug.Log($"!!!!{turn.move}");
            }
        }*/

        if (hasKing) 
        {
            KingDangerSquares.Add(kingDanger);
            if (hasPin)
            {
                Pinning = enemieInBetween;
                Pinning.PinnedBy = this;
                PinSquares.Add(pinSquares);
            }
        }
    }
}
