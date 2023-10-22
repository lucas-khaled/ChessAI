using System;
using System.Collections.Generic;

public class Pawn : BlockableMovesPiece
{
    public override Move[] GetMoves(Board board)
    {
        List<Move> possibleMoves = new List<Move>();

        possibleMoves.AddRange(GetFowardMoves(board));
        possibleMoves.AddRange(GetCaptures(board));

        var enPassant = GetEnPassant();
        if (enPassant != null)
            possibleMoves.Add(enPassant);
        
        return possibleMoves.ToArray();
    }

    private Move[] GetFowardMoves(Board board) 
    {
        int range = (IsOnInitialRow()) ? 2 : 1;

        var verticals = GameManager.BoardManager.GetVerticalsFrom(board, actualTile.TilePosition, pieceColor, range);
        var checkingBlockVerticals = CheckForBlockingSquares(verticals.frontVerticals, false);
        
        return CreateMovesFromSegment(checkingBlockVerticals);
    }

    private bool IsOnInitialRow()
    {
        return (Row == 1 && IsWhite)
            || (Row == 6 && !IsWhite);
    }

    private Move[] GetCaptures(Board board) 
    {
        List<Move> moves = new();

        var diagonals = GameManager.BoardManager.GetDiagonalsFrom(board, actualTile.TilePosition, pieceColor, 1);

        if(CanMoveToDiagonal(diagonals.topLeftDiagonals))
            moves.AddRange(CreateMovesFromSegment(diagonals.topLeftDiagonals));

        if (CanMoveToDiagonal(diagonals.topRightDiagonals))
            moves.AddRange(CreateMovesFromSegment(diagonals.topRightDiagonals));

        return moves.ToArray();
    }

    private bool CanMoveToDiagonal(List<Tile> diagonal) 
    {
        return diagonal.Count > 0 && IsEnemyPiece(diagonal[0].OccupiedBy);
    }

    private Move GetEnPassant()
    {
        if (IsInEnPassantRow() is false) return null;

        var lastDestiny = MoveMaker.LastMove.to;
        if (lastDestiny.OccupiedBy is not Pawn enemyPawn) return null;

        int rowForward = (pieceColor == PieceColor.White) ? 1 : -1;

        bool isSameRow = lastDestiny.TilePosition.row == actualTile.TilePosition.row;
        bool isPassedRow = lastDestiny.TilePosition.row == actualTile.TilePosition.row - rowForward;
        if (isSameRow is false && isPassedRow is false) return null;

        int columnDelta = lastDestiny.TilePosition.column - actualTile.TilePosition.column;
        bool isAdjacentColumn = Math.Abs(columnDelta) == 1;
        if (isAdjacentColumn is false) return null;

        
        var destinyTile = GameManager.Board.GetTiles()[actualTile.TilePosition.row + rowForward][lastDestiny.TilePosition.column];
        return new Move(actualTile, destinyTile, enemyPawn);
    }

    private bool IsInEnPassantRow() 
    {
        var row = actualTile.TilePosition.row;
        return (row == 4 || row == 5 && pieceColor == PieceColor.White)
            || (row == 3 || row == 2 && pieceColor == PieceColor.Black);
    }
}