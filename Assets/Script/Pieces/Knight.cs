using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public Knight(Environment env) : base(env)
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

        var horizontals = Environment.boardManager.GetHorizontalsFrom(actualTile.TilePosition, pieceColor, 2);

        moves.AddRange(GetMovesFromHorizontal(horizontals.rightHorizontals));
        moves.AddRange(GetMovesFromHorizontal(horizontals.leftHorizontals));

        return moves;
    }

    private List<Move> GetMovesFromHorizontal(List<Tile> horizontal) 
    {
        List<Move> moves = new List<Move>();

        if (horizontal.Count == 2)
        {
            var edge = horizontal[1];
            var edgeVerticals = Environment.boardManager.GetVerticalsFrom(edge.TilePosition, pieceColor, 1);

            var checkedFront = CheckForBlockingSquares(edgeVerticals.frontVerticals);
            var checkedBack = CheckForBlockingSquares(edgeVerticals.backVerticals);

            moves.AddRange(CreateMovesFromSegment(checkedFront));
            moves.AddRange(CreateMovesFromSegment(checkedBack));
        }

        return moves;
    }

    private List<Move> GetMovesFromVertical()
    {
        List<Move> moves = new List<Move>();

        var verticals = Environment.boardManager.GetVerticalsFrom(actualTile.TilePosition, pieceColor, 2);

        moves.AddRange(GetMovesFromVertical(verticals.frontVerticals));
        moves.AddRange(GetMovesFromVertical(verticals.backVerticals));

        return moves;
    }

    private List<Move> GetMovesFromVertical(List<Tile> vertical)
    {
        List<Move> moves = new List<Move>();

        if (vertical.Count == 2)
        {
            var edge = vertical[1];
            var edgeHorizontals = Environment.boardManager.GetHorizontalsFrom(edge.TilePosition, pieceColor, 1);

            var checkedLeft = CheckForBlockingSquares(edgeHorizontals.leftHorizontals);
            var checkedRight = CheckForBlockingSquares(edgeHorizontals.rightHorizontals);

            moves.AddRange(CreateMovesFromSegment(checkedLeft));
            moves.AddRange(CreateMovesFromSegment(checkedRight));
        }

        return moves;
    }
}
