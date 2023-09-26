using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlockableMovesPiece : Piece
{
    protected List<Move> GetVerticalMoves(int range = 8)
    {
        List<Move> moves = new();

        var verticals = GameManager.Board.GetVerticalsFrom(actualTile.TilePosition, pieceColor, range);

        var checkedFrontBlockingSquares = CheckForBlockingSquares(verticals.frontVerticals);
        var checkedBackBlockingSquares = CheckForBlockingSquares(verticals.backVerticals);

        moves.AddRange(CreateMovesFromSegment(checkedFrontBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedBackBlockingSquares));

        return moves;
    }

    protected List<Move> GetHorizontalMoves(int range = 8)
    {
        List<Move> moves = new();

        var horizontals = GameManager.Board.GetHorizontalsFrom(actualTile.TilePosition, pieceColor, range);

        var checkedLeftBlockingSquares = CheckForBlockingSquares(horizontals.leftHorizontals);
        var checkedRightBlockingSquares = CheckForBlockingSquares(horizontals.rightHorizontals);

        moves.AddRange(CreateMovesFromSegment(checkedLeftBlockingSquares));
        moves.AddRange(CreateMovesFromSegment(checkedRightBlockingSquares));

        return moves;
    }

    protected List<Move> GetDiagonalMoves(int range = 8) 
    {
        List<Move> moves = new();

        var diagonals = GameManager.Board.GetDiagonalsFrom(actualTile.TilePosition, pieceColor, range);

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
}
