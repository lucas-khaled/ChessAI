using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override Move[] GetMoves(Board board)
    {
        List<Move> moves = new List<Move>();

        moves.AddRange(GetMovesFromHorizontals(board));
        moves.AddRange(GetMovesFromVertical(board));

        return moves.ToArray();
    }

    private List<Move> GetMovesFromHorizontals(Board board) 
    {
        List<Move> moves = new List<Move>();

        var horizontals = GameManager.BoardManager.GetHorizontalsFrom(board, actualTile.TilePosition, pieceColor, 2);

        moves.AddRange(GetMovesFromHorizontal(board, horizontals.rightHorizontals));
        moves.AddRange(GetMovesFromHorizontal(board, horizontals.leftHorizontals));

        return moves;
    }

    private List<Move> GetMovesFromHorizontal(Board board, List<Tile> horizontal) 
    {
        List<Move> moves = new List<Move>();

        if (horizontal.Count == 2)
        {
            var edge = horizontal[1];
            var edgeVerticals = GameManager.BoardManager.GetVerticalsFrom(board, edge.TilePosition, pieceColor, 1);

            var checkedFront = CheckForBlockingSquares(edgeVerticals.frontVerticals);
            var checkedBack = CheckForBlockingSquares(edgeVerticals.backVerticals);

            moves.AddRange(CreateMovesFromSegment(checkedFront));
            moves.AddRange(CreateMovesFromSegment(checkedBack));
        }

        return moves;
    }

    private List<Move> GetMovesFromVertical(Board board)
    {
        List<Move> moves = new List<Move>();

        var verticals = GameManager.BoardManager.GetVerticalsFrom(board, actualTile.TilePosition, pieceColor, 2);

        moves.AddRange(GetMovesFromVertical(board, verticals.frontVerticals));
        moves.AddRange(GetMovesFromVertical(board, verticals.backVerticals));

        return moves;
    }

    private List<Move> GetMovesFromVertical(Board board,List<Tile> vertical)
    {
        List<Move> moves = new List<Move>();

        if (vertical.Count == 2)
        {
            var edge = vertical[1];
            var edgeHorizontals = GameManager.BoardManager.GetHorizontalsFrom(board, edge.TilePosition, pieceColor, 1);

            var checkedLeft = CheckForBlockingSquares(edgeHorizontals.leftHorizontals);
            var checkedRight = CheckForBlockingSquares(edgeHorizontals.rightHorizontals);

            moves.AddRange(CreateMovesFromSegment(checkedLeft));
            moves.AddRange(CreateMovesFromSegment(checkedRight));
        }

        return moves;
    }
}
