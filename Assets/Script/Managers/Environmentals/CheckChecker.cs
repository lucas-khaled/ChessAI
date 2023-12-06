using System.Collections.Generic;
using System.Linq;

public class CheckChecker
{
    Verticals verticals;
    Horizontals horizontals;
    Diagonals diagonals;
    MoveCheckmark checkMarkBools;
    PieceColor colorTurn;
    Environment environment;
    Tile kingTile;

    public bool IsCheck(Environment env, PieceColor colorTurn)
    {
        Setup(env, colorTurn);
        
        for (int i = 0; i < 8; i++)
        {
            if(ThereIsCheckOnVerticals(i))
                return true;

            if (ThereIsCheckOnHorizontals(i))
                return true;

            if (ThereIsCheckOnDiagonals(i))
                return true;
        }

        return ThereIsKnightCheck();
    }

    private void Setup(Environment env, PieceColor colorTurn) 
    {
        environment = env;
        var manager = env.boardManager;

        kingTile = manager.GetKingTile(colorTurn);
        verticals = manager.GetVerticalsFrom(kingTile.TilePosition, colorTurn);
        horizontals = manager.GetHorizontalsFrom(kingTile.TilePosition, colorTurn);
        diagonals = manager.GetDiagonalsFrom(kingTile.TilePosition, colorTurn);

        checkMarkBools = new();
        this.colorTurn = colorTurn;
    }

    private bool ThereIsCheckOnVerticals(int i) 
    {
        return ThereIsRookCheck(i, verticals.backVerticals, ref checkMarkBools.vertD) || ThereIsRookCheck(i, verticals.frontVerticals, ref checkMarkBools.vertU);
    }

    private bool ThereIsCheckOnHorizontals(int i) 
    {
        return ThereIsRookCheck(i, horizontals.leftHorizontals, ref checkMarkBools.horL) || ThereIsRookCheck(i, horizontals.rightHorizontals, ref checkMarkBools.horR);
    }

    private bool ThereIsRookCheck(int index, List<Tile> segment, ref bool checkmark)
    {
        if (checkmark is false && segment.Count > index && segment[index].IsOccupied)
        {
            Piece piece = segment[index].OccupiedBy;
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

    private bool ThereIsBishopCheck(int index, List<Tile> segment, ref bool checkmark, bool isTopDiagonal = false)
    {
        if (checkmark is false && segment.Count > index && segment[index].IsOccupied)
        {
            Piece piece = segment[index].OccupiedBy;
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

    private bool ThereIsKnightCheck() 
    {
        Knight knight = new(environment);
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
