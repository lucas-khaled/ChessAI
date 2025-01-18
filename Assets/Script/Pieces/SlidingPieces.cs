using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlidingPieces : Piece
{
    protected SlidingPieces(Board board) : base(board)
    {
    }

    protected Bitboard GetVerticalBlockedSquares(int range = 8, bool includeFriendlySquare = true) 
    {
        Bitboard bitboard = new();
        var verticals = actualTile.GetVerticalsByColor(pieceColor);

        var rangedFront = verticals.frontVerticals.GetRange(0, Mathf.Min(range, verticals.frontVerticals.Count));
        var checkedFrontBlockingSquares = GetBitboardUntilBlockedSquare(rangedFront, includeBlockingPieceSquare: includeFriendlySquare);

        var rangedBack = verticals.backVerticals.GetRange(0, Mathf.Min(range, verticals.backVerticals.Count));
        var checkedBackBlockingSquares = GetBitboardUntilBlockedSquare(rangedBack, includeBlockingPieceSquare: includeFriendlySquare);

        bitboard.Add(checkedFrontBlockingSquares);
        bitboard.Add(checkedBackBlockingSquares);

        return bitboard;
    }

    protected Bitboard GetHorizontalBlockedSquares(int range = 8, bool includeFriendlySquare = true)
    {
        Bitboard bitboard = new();
        var horizontals = actualTile.GetHorizontalsByColor(pieceColor);

        var rangedLeft = horizontals.leftHorizontals.GetRange(0, Mathf.Min(range, horizontals.leftHorizontals.Count));
        var checkedLeftBlockingSquares = GetBitboardUntilBlockedSquare(rangedLeft, includeBlockingPieceSquare: includeFriendlySquare);

        var rangedRight = horizontals.rightHorizontals.GetRange(0, Mathf.Min(range, horizontals.rightHorizontals.Count));
        var checkedRightBlockingSquares = GetBitboardUntilBlockedSquare(rangedRight, includeBlockingPieceSquare: includeFriendlySquare);

        bitboard.Add(checkedLeftBlockingSquares);
        bitboard.Add(checkedRightBlockingSquares);

        return bitboard;
    }

    protected Bitboard GetDiagonalBlockedSquares(int range = 8, bool includeFriendlySquare = true) 
    {
        Bitboard bitboard = new();

        var diagonals = actualTile.GetDiagonalsByColor(pieceColor);

        var rangedTopLeft = diagonals.topLeftDiagonals.GetRange(0, Mathf.Min(range, diagonals.topLeftDiagonals.Count));
        var checkedTopLeftBlockingSquares = GetBitboardUntilBlockedSquare(rangedTopLeft, includeBlockingPieceSquare: includeFriendlySquare);

        var rangedTopRight = diagonals.topRightDiagonals.GetRange(0, Mathf.Min(range, diagonals.topRightDiagonals.Count));
        var checkedTopRightBlockingSquares = GetBitboardUntilBlockedSquare(rangedTopRight, includeBlockingPieceSquare: includeFriendlySquare);

        var rangedDownLeft = diagonals.downLeftDiagonals.GetRange(0, Mathf.Min(range, diagonals.downLeftDiagonals.Count));
        var checkedDownLeftBlockingSquares = GetBitboardUntilBlockedSquare(rangedDownLeft, includeBlockingPieceSquare: includeFriendlySquare);

        var rangedDownRight = diagonals.downRightDiagonals.GetRange(0, Mathf.Min(range, diagonals.downRightDiagonals.Count));
        var checkedDownRightBlockingSquares = GetBitboardUntilBlockedSquare(rangedDownRight, includeBlockingPieceSquare: includeFriendlySquare);

        bitboard.Add(checkedTopLeftBlockingSquares);
        bitboard.Add(checkedTopRightBlockingSquares);
        bitboard.Add(checkedDownLeftBlockingSquares);
        bitboard.Add(checkedDownRightBlockingSquares);

        return bitboard;
    }
}
