using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public override Move[] GetPossibleMoves()
    {
        List<Move> moves = new();
        moves.AddRange(GetVerticalMoves());
        moves.AddRange(GetHorizontalMoves());

        return moves.ToArray();
    }

    private List<Move> GetVerticalMoves() 
    {
        List<Move> moves = new();

        var verticals = GameManager.Board.GetVerticalTilesFrom(actualTile.TilePosition, pieceColor);

        var checkedFrontBlockingSquares = CheckForBlockingSquares(verticals.frontVerticals);
        var checkedBackBlockingSquares = CheckForBlockingSquares(verticals.backVerticals);

        moves.AddRange(CreateMovesFromSegment(checkedFrontBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedBackBlockingSquares));

        return moves;
    }

    private List<Move> GetHorizontalMoves()
    {
        List<Move> moves = new();

        var horizontals = GameManager.Board.GetHorizontalTilesFrom(actualTile.TilePosition, pieceColor);

        var checkedLeftBlockingSquares = CheckForBlockingSquares(horizontals.leftHorizontals);
        var checkedRightBlockingSquares = CheckForBlockingSquares(horizontals.rightHorizontals);

        moves.AddRange(CreateMovesFromSegment(checkedLeftBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedRightBlockingSquares));

        return moves;
    }

    private List<Tile> CheckForBlockingSquares(List<Tile> segment) 
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

    private Move[] CreateMovesFromSegment(List<Tile> segments) 
    {
        Move[] moves = new Move[segments.Count];

        for(int i = 0; i<segments.Count; i++) 
        {
            var capture = (IsEnemyPiece(segments[i].OccupiedBy)) ? segments[i].OccupiedBy : null;

            moves[i] = new Move(actualTile, segments[i], capture);
        }

        return moves;
    }
}
