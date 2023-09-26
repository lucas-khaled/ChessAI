using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlockableMovesPiece : Piece
{
    protected List<Move> GetVerticalMoves()
    {
        List<Move> moves = new();

        var verticals = GameManager.Board.GetVerticalTilesFrom(actualTile.TilePosition, pieceColor);

        var checkedFrontBlockingSquares = CheckForBlockingSquares(verticals.frontVerticals);
        var checkedBackBlockingSquares = CheckForBlockingSquares(verticals.backVerticals);

        moves.AddRange(CreateMovesFromSegment(checkedFrontBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedBackBlockingSquares));

        return moves;
    }

    protected List<Move> GetHorizontalMoves()
    {
        List<Move> moves = new();

        var horizontals = GameManager.Board.GetHorizontalTilesFrom(actualTile.TilePosition, pieceColor);

        var checkedLeftBlockingSquares = CheckForBlockingSquares(horizontals.leftHorizontals);
        var checkedRightBlockingSquares = CheckForBlockingSquares(horizontals.rightHorizontals);

        moves.AddRange(CreateMovesFromSegment(checkedLeftBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedRightBlockingSquares));

        return moves;
    }

    protected List<Move> GetDiagonalMoves() 
    {
        List<Move> moves = new();

        var diagonals = GameManager.Board.GetDiagonalsFrom(actualTile.TilePosition, pieceColor);

        var checkedTopLeftBlockingSquares = CheckForBlockingSquares(diagonals.topLeftDiagonals);
        var checkedTopRightBlockingSquares = CheckForBlockingSquares(diagonals.topRightDiagonals);
        var checkedDownLeftBlockingSquares = CheckForBlockingSquares(diagonals.downLeftDiagonals);
        var checkedDownRightBlockingSquares = CheckForBlockingSquares(diagonals.downRightDiagonals);

        moves.AddRange(CreateMovesFromSegment(checkedTopLeftBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedTopRightBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedDownLeftBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedDownRightBlockingSquares));

        return moves;
    }

    protected List<Tile> CheckForBlockingSquares(List<Tile> segment)
    {
        List<Tile> finalTiles = new();
        foreach (var tile in segment)
        {
            if (tile.IsOccupied)
            {
                if (IsEnemyPiece(tile.OccupiedBy))
                    finalTiles.Add(tile);

                break;
            }

            finalTiles.Add(tile);
        }

        return finalTiles;
    }

    protected Move[] CreateMovesFromSegment(List<Tile> segments)
    {
        Move[] moves = new Move[segments.Count];

        for (int i = 0; i < segments.Count; i++)
        {
            var capture = (IsEnemyPiece(segments[i].OccupiedBy)) ? segments[i].OccupiedBy : null;

            moves[i] = new Move(actualTile, segments[i], capture);
        }

        return moves;
    }
}
