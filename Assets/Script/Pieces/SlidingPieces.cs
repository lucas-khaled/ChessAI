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

        var rangedFront = verticals.frontVerticals.GetRange(0, Mathf.Min(range, verticals.frontVerticals.Count));
        var checkedFrontBlockingSquares = CheckForBlockingSquares(rangedFront);

        var rangedBack = verticals.backVerticals.GetRange(0, Mathf.Min(range, verticals.backVerticals.Count));
        var checkedBackBlockingSquares = CheckForBlockingSquares(rangedBack);

        moves.AddRange(CreateMovesFromSegment(checkedFrontBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedBackBlockingSquares));

        return moves;
    }

    protected List<Tile> GetVerticalBlockedSquares(int range = 8) 
    {
        List<Tile> tiles = new();
        var verticals = actualTile.GetVerticalsByColor(pieceColor);

        var rangedFront = verticals.frontVerticals.GetRange(0, Mathf.Min(range, verticals.frontVerticals.Count));
        var checkedFrontBlockingSquares = CheckForBlockingSquares(rangedFront);

        var rangedBack = verticals.backVerticals.GetRange(0, Mathf.Min(range, verticals.backVerticals.Count));
        var checkedBackBlockingSquares = CheckForBlockingSquares(rangedBack);

        tiles.AddRange(checkedFrontBlockingSquares);
        tiles.AddRange(checkedBackBlockingSquares);

        return tiles;
    }

    protected List<Move> GetHorizontalMoves(int range = 8, bool includeFriendlySquare = false)
    {
        List<Move> moves = new();

        var horizontals = actualTile.GetHorizontalsByColor(pieceColor);

        var rangedLeft = horizontals.leftHorizontals.GetRange(0, Mathf.Min(range, horizontals.leftHorizontals.Count));
        var checkedLeftBlockingSquares = CheckForBlockingSquares(rangedLeft, includeBlockingPieceSquare: includeFriendlySquare);

        var rangedRight = horizontals.rightHorizontals.GetRange(0, Mathf.Min(range, horizontals.rightHorizontals.Count));
        var checkedRightBlockingSquares = CheckForBlockingSquares(rangedRight, includeBlockingPieceSquare: includeFriendlySquare);

        moves.AddRange(CreateMovesFromSegment(checkedLeftBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedRightBlockingSquares));

        return moves;
    }

    protected List<Tile> GetHorizontalBlockedSquares(int range = 8, bool includeFriendlySquare = false)
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

    protected List<Tile> GetDiagonalBlockedSquares(int range = 8, bool includeFriendlySquare = false) 
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

    protected List<Tile> GetKingDangerValidSegment(List<TileCoordinates> segment) 
    {
        List<Tile> finalTiles = new();
        bool hasKing = false;
        int enemiesInBetween = 0;
        foreach (var tileCoord in segment)
        {
            Tile tile = Board.tiles[tileCoord.row][tileCoord.column];
            if (tile.IsOccupied && tile.OccupiedBy.pieceColor == pieceColor) break;

            if (tile.IsOccupied)
            {
                if (tile.OccupiedBy is King)
                    hasKing = true;

                enemiesInBetween++;
                if (enemiesInBetween > 1) break;
            }

            finalTiles.Add(tile);
        }

        return hasKing ? finalTiles : null;
    }
}
