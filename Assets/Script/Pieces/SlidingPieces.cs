using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlidingPieces : Piece
{
    protected SlidingPieces(Board board) : base(board)
    {
    }

    protected List<Move> GetVerticalMoves(int range = 8)
    {
        List<Move> moves = new();

        var verticals = actualTile.GetVerticalsByColor(pieceColor);

        var checkedFrontBlockingSquares = CheckForBlockingSquares(verticals.frontVerticals.GetRange(0, Mathf.Min(range, verticals.frontVerticals.Count)));
        var checkedBackBlockingSquares = CheckForBlockingSquares(verticals.backVerticals.GetRange(0, Mathf.Min(range, verticals.backVerticals.Count)));

        moves.AddRange(CreateMovesFromSegment(checkedFrontBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedBackBlockingSquares));

        return moves;
    }

    protected List<Move> GetHorizontalMoves(int range = 8)
    {
        List<Move> moves = new();

        var horizontals = actualTile.GetHorizontalsByColor(pieceColor);

        var checkedLeftBlockingSquares = CheckForBlockingSquares(horizontals.leftHorizontals.GetRange(0, Mathf.Min(range, horizontals.leftHorizontals.Count)));
        var checkedRightBlockingSquares = CheckForBlockingSquares(horizontals.rightHorizontals.GetRange(0, Mathf.Min(range, horizontals.rightHorizontals.Count)));

        moves.AddRange(CreateMovesFromSegment(checkedLeftBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedRightBlockingSquares));

        return moves;
    }

    protected List<Move> GetDiagonalMoves(int range = 8) 
    {
        List<Move> moves = new();

        var diagonals = actualTile.GetDiagonalsByColor(pieceColor);

        var checkedTopLeftBlockingSquares = CheckForBlockingSquares(diagonals.topLeftDiagonals.GetRange(0, Mathf.Min(range, diagonals.topLeftDiagonals.Count))) ;
        var checkedTopRightBlockingSquares = CheckForBlockingSquares(diagonals.topRightDiagonals.GetRange(0, Mathf.Min(range, diagonals.topRightDiagonals.Count)));
        var checkedDownLeftBlockingSquares = CheckForBlockingSquares(diagonals.downLeftDiagonals.GetRange(0, Mathf.Min(range, diagonals.downLeftDiagonals.Count)));
        var checkedDownRightBlockingSquares = CheckForBlockingSquares(diagonals.downRightDiagonals.GetRange(0, Mathf.Min(range, diagonals.downRightDiagonals.Count)));

        moves.AddRange(CreateMovesFromSegment(checkedTopLeftBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedTopRightBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedDownLeftBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedDownRightBlockingSquares));

        return moves;
    }
}
