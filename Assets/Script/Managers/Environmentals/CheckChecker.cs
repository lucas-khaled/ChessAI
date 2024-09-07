using System.Collections.Generic;
using System.Linq;

public class CheckChecker
{
    Verticals verticals;
    Horizontals horizontals;
    Diagonals diagonals;
    MoveCheckmark checkMarkBools;
    PieceColor colorTurn;
    Tile kingTile;
    Board board;

    public bool IsCheck(Board board, PieceColor colorTurn)
    {
        Setup(board, colorTurn);
        
        for (int i = 0; i < 8; i++)
        {
            if(ThereIsCheckOnVerticals(i))
                return true;

            if (ThereIsCheckOnHorizontals(i))
                return true;

            if (ThereIsCheckOnDiagonals(i))
                return true;
        }

        return ThereIsKnightCheck(board);
    }

    private void Setup(Board board, PieceColor colorTurn) 
    {
        kingTile = board.GetKingTile(colorTurn);
        verticals = kingTile.GetVerticalsByColor(colorTurn);
        horizontals = kingTile.GetHorizontalsByColor(colorTurn);
        diagonals = kingTile.GetDiagonalsByColor(colorTurn);

        checkMarkBools = new();
        this.colorTurn = colorTurn;

        this.board = board;
    }

    private bool ThereIsCheckOnVerticals(int i) 
    {
        return ThereIsRookCheck(i, verticals.backVerticals, ref checkMarkBools.vertD) || ThereIsRookCheck(i, verticals.frontVerticals, ref checkMarkBools.vertU);
    }

    private bool ThereIsCheckOnHorizontals(int i) 
    {
        return ThereIsRookCheck(i, horizontals.leftHorizontals, ref checkMarkBools.horL) || ThereIsRookCheck(i, horizontals.rightHorizontals, ref checkMarkBools.horR);
    }

    private bool ThereIsRookCheck(int index, List<TileCoordinates> segment, ref bool checkmark)
    {
        if (segment.Count <= index) return false;

        TileCoordinates coord = segment[index];
        Tile tile = board.tiles[coord.row][coord.column];
        if (checkmark is false && tile.IsOccupied)
        {
            Piece piece = tile.OccupiedBy;
            if (piece.pieceColor == colorTurn)
                checkmark = true;
            else
            {
                if (piece is Rook || piece is Queen || (piece is King && index == 0))
                    return true;

                checkmark = true;
            }
        }

        return false;
    }

    private bool ThereIsCheckOnDiagonals(int i) 
    {
        return ThereIsBishopCheck(i, diagonals.topLeftDiagonals, ref checkMarkBools.diagUL, true) || 
            ThereIsBishopCheck(i, diagonals.topRightDiagonals, ref checkMarkBools.diagUR, true) ||
            ThereIsBishopCheck(i, diagonals.downLeftDiagonals, ref checkMarkBools.diagDL) || 
            ThereIsBishopCheck(i, diagonals.downRightDiagonals, ref checkMarkBools.diagDR);
    }

    private bool ThereIsBishopCheck(int index, List<TileCoordinates> segment, ref bool checkmark, bool isTopDiagonal = false)
    {
        if (segment.Count <= index) return false;

        TileCoordinates coord = segment[index];
        Tile tile = board.tiles[coord.row][coord.column];

        if (checkmark is false && tile.IsOccupied)
        {
            Piece piece = tile.OccupiedBy;
            if (piece.pieceColor == colorTurn)
                checkmark = true;
            else
            {
                if ((isTopDiagonal && piece is Pawn && index == 0) || piece is Bishop || piece is Queen || (piece is King && index == 0))
                    return true;

                checkmark = true;
            }
        }

        return false;
    }

    private bool ThereIsKnightCheck(Board board) 
    {
        Knight knight = new(board);
        knight.SetTile(kingTile);
        knight.pieceColor = colorTurn;

        return knight.GetMoves().Any(m => m.to.OccupiedBy is Knight);
    }

    struct MoveCheckmark
    {
        public bool horR;
        public bool horL;
        public bool vertU;
        public bool vertD;
        public bool diagDL;
        public bool diagDR;
        public bool diagUL;
        public bool diagUR;
    }
}
