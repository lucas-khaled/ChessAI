using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public Knight(Board board) : base(board)
    {
    }

    public override Move[] GetMoves()
    {
        List<Move> moves = new List<Move>();
        moves.AddRange(GetMovesFromHorizontals());
        moves.AddRange(GetMovesFromVertical());

        return moves.ToArray();
    }

    private List<Move> GetMovesFromHorizontals() 
    {
        List<Move> moves = new List<Move>();

        var horizontals = actualTile.GetHorizontalsByColor(pieceColor);
        moves.AddRange(GetMovesFromHorizontal(horizontals.rightHorizontals));
        moves.AddRange(GetMovesFromHorizontal(horizontals.leftHorizontals));

        return moves;
    }

    private List<Move> GetMovesFromHorizontal(List<TileCoordinates> horizontal) 
    {
        List<Move> moves = new List<Move>();

        if (horizontal.Count >= 2)
        {
            var edgeCoord = horizontal[1];
            var edge = Board.tiles[edgeCoord.row][edgeCoord.column];
            var edgeVerticals = edge.GetVerticalsByColor(pieceColor);

            if (edgeVerticals.frontVerticals.Count > 0)
            {
                var checkedFront = CheckForBlockingSquares(edgeVerticals.frontVerticals.GetRange(0, 1));
                moves.AddRange(CreateMovesFromSegment(checkedFront));
            }

            if (edgeVerticals.backVerticals.Count > 0)
            {
                var checkedBack = CheckForBlockingSquares(edgeVerticals.backVerticals.GetRange(0, 1));
                moves.AddRange(CreateMovesFromSegment(checkedBack));
            }
        }

        return moves;
    }

    private List<Move> GetMovesFromVertical()
    {
        List<Move> moves = new List<Move>();

        var verticals = actualTile.GetVerticalsByColor(pieceColor);

        moves.AddRange(GetMovesFromVertical(verticals.frontVerticals));
        moves.AddRange(GetMovesFromVertical(verticals.backVerticals));

        return moves;
    }

    private List<Move> GetMovesFromVertical(List<TileCoordinates> vertical)
    {
        List<Move> moves = new List<Move>();

        if (vertical.Count >= 2)
        {
            var edgeCoord = vertical[1];
            var edge = Board.tiles[edgeCoord.row][edgeCoord.column];
            var edgeHorizontals = edge.GetHorizontalsByColor(pieceColor);

            if (edgeHorizontals.leftHorizontals.Count > 0)
            {
                var checkedLeft = CheckForBlockingSquares(edgeHorizontals.leftHorizontals.GetRange(0, 1));
                moves.AddRange(CreateMovesFromSegment(checkedLeft));
            }

            if (edgeHorizontals.rightHorizontals.Count > 0)
            {
                var checkedRight = CheckForBlockingSquares(edgeHorizontals.rightHorizontals.GetRange(0, 1));
                moves.AddRange(CreateMovesFromSegment(checkedRight));
            }
        }

        return moves;
    }
}
