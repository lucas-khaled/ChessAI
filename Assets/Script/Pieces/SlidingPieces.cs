using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlidingPieces : Piece
{
    protected SlidingPieces(Board board) : base(board)
    {
    }

    protected List<Tile> GetVerticalBlockedSquares(int range = 8, bool includeFriendlySquare = true) 
    {
        List<Tile> tiles = new();
        var verticals = actualTile.GetVerticalsByColor(pieceColor);

        var rangedFront = verticals.frontVerticals.GetRange(0, Mathf.Min(range, verticals.frontVerticals.Count));
        var checkedFrontBlockingSquares = CheckForBlockingSquares(rangedFront, includeBlockingPieceSquare: includeFriendlySquare);

        var rangedBack = verticals.backVerticals.GetRange(0, Mathf.Min(range, verticals.backVerticals.Count));
        var checkedBackBlockingSquares = CheckForBlockingSquares(rangedBack, includeBlockingPieceSquare: includeFriendlySquare);

        tiles.AddRange(checkedFrontBlockingSquares);
        tiles.AddRange(checkedBackBlockingSquares);

        return tiles;
    }

    protected List<Tile> GetHorizontalBlockedSquares(int range = 8, bool includeFriendlySquare = true)
    {
        List<Tile> tiles = new();
        var horizontals = actualTile.GetHorizontalsByColor(pieceColor);

        var rangedLeft = horizontals.leftHorizontals.GetRange(0, Mathf.Min(range, horizontals.leftHorizontals.Count));
        var checkedLeftBlockingSquares = CheckForBlockingSquares(rangedLeft, includeBlockingPieceSquare: includeFriendlySquare);

        var rangedRight = horizontals.rightHorizontals.GetRange(0, Mathf.Min(range, horizontals.rightHorizontals.Count));
        var checkedRightBlockingSquares = CheckForBlockingSquares(rangedRight, includeBlockingPieceSquare: includeFriendlySquare);

        tiles.AddRange(checkedLeftBlockingSquares);
        tiles.AddRange(checkedRightBlockingSquares);

        return tiles;
    }

    protected List<Tile> GetDiagonalBlockedSquares(int range = 8, bool includeFriendlySquare = true) 
    {
        List<Tile> tiles = new();

        var diagonals = actualTile.GetDiagonalsByColor(pieceColor);

        var rangedTopLeft = diagonals.topLeftDiagonals.GetRange(0, Mathf.Min(range, diagonals.topLeftDiagonals.Count));
        var checkedTopLeftBlockingSquares = CheckForBlockingSquares(rangedTopLeft, includeBlockingPieceSquare: includeFriendlySquare);

        var rangedTopRight = diagonals.topRightDiagonals.GetRange(0, Mathf.Min(range, diagonals.topRightDiagonals.Count));
        var checkedTopRightBlockingSquares = CheckForBlockingSquares(rangedTopRight, includeBlockingPieceSquare: includeFriendlySquare);

        var rangedDownLeft = diagonals.downLeftDiagonals.GetRange(0, Mathf.Min(range, diagonals.downLeftDiagonals.Count));
        var checkedDownLeftBlockingSquares = CheckForBlockingSquares(rangedDownLeft, includeBlockingPieceSquare: includeFriendlySquare);

        var rangedDownRight = diagonals.downRightDiagonals.GetRange(0, Mathf.Min(range, diagonals.downRightDiagonals.Count));
        var checkedDownRightBlockingSquares = CheckForBlockingSquares(rangedDownRight, includeBlockingPieceSquare: includeFriendlySquare);

        tiles.AddRange(checkedTopLeftBlockingSquares);
        tiles.AddRange(checkedTopRightBlockingSquares);
        tiles.AddRange(checkedDownLeftBlockingSquares);
        tiles.AddRange(checkedDownRightBlockingSquares);

        return tiles;
    }
}
