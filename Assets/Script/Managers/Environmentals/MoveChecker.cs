using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveChecker : IEnvironmentable
{
    public Environment Environment { get; }

    public MoveChecker(Environment env) 
    {
        Environment = env;
    }

    public IEnvironmentable Copy(Environment env)
    {
        return new MoveChecker(env);
    }

    public Move[] GetLegalMoves(Move[] moves)
    {
        var returningMoves = FilterCheckMoves(moves);

        return returningMoves.ToArray();
    }

    private List<Move> FilterCheckMoves(Move[] moves)
    {
        List<Move> validMoves = new List<Move>();
        PieceColor turn = Environment.turnManager.ActualTurn;
        foreach (var move in moves)
        {
            var env = Environment.Copy();
            env.turnManager.SetMove(move);

            if (IsCheck(env, turn) is false)
                validMoves.Add(move);
        }

        return validMoves;
    }

    public bool IsCheck(Environment env, PieceColor colorTurn)
    {
        var manager = env.boardManager;
        var board = env.board;

        Tile kingTile = board.GetKingTile(colorTurn);
        Verticals vert = manager.GetVerticalsFrom(kingTile.TilePosition, colorTurn);
        Horizontals hor = manager.GetHorizontalsFrom(kingTile.TilePosition, colorTurn);
        Diagonals diag = manager.GetDiagonalsFrom(kingTile.TilePosition, colorTurn);

        MoveChecking checkingBools = new();

        for (int i = 0; i < 8; i++)
        {
            if (checkingBools.vertD is false && vert.backVerticals.Count > i && vert.backVerticals[i].IsOccupied)
            {
                Piece piece = vert.backVerticals[i].OccupiedBy;
                if (piece.pieceColor == colorTurn)
                    checkingBools.vertD = true;
                else
                {
                    if (piece is Rook || piece is Queen)
                        return true;

                    checkingBools.vertD = true;
                }
            }

            if (checkingBools.vertU is false && vert.frontVerticals.Count > i && vert.frontVerticals[i].IsOccupied)
            {
                Piece piece = vert.frontVerticals[i].OccupiedBy;
                if (piece.pieceColor == colorTurn)
                    checkingBools.vertU = true;
                else
                {
                    if (piece is Rook || piece is Queen)
                        return true;

                    checkingBools.vertU = true;
                }
            }

            if (checkingBools.horL is false && hor.leftHorizontals.Count > i && hor.leftHorizontals[i].IsOccupied)
            {
                Piece piece = hor.leftHorizontals[i].OccupiedBy;
                if (piece.pieceColor == colorTurn)
                    checkingBools.horL = true;
                else
                {
                    if (piece is Rook || piece is Queen)
                        return true;

                    checkingBools.horL = true;
                }
            }

            if (checkingBools.horR is false && hor.rightHorizontals.Count > i && hor.rightHorizontals[i].IsOccupied)
            {
                Piece piece = hor.rightHorizontals[i].OccupiedBy;
                if (piece.pieceColor == colorTurn)
                    checkingBools.horR = true;
                else
                {
                    if (piece is Rook || piece is Queen)
                        return true;

                    checkingBools.horR = true;
                }
            }

            if (checkingBools.diagUL is false && diag.topLeftDiagonals.Count > i && diag.topLeftDiagonals[i].IsOccupied)
            {
                Piece piece = diag.topLeftDiagonals[i].OccupiedBy;
                if (piece.pieceColor == colorTurn)
                    checkingBools.diagUL = true;
                else
                {
                    if (piece is Bishop || piece is Queen || (piece is Pawn && i == 0))
                        return true;

                    checkingBools.diagUL = true;
                }
            }

            if (checkingBools.diagUR is false && diag.topRightDiagonals.Count > i && diag.topRightDiagonals[i].IsOccupied)
            {
                Piece piece = diag.topRightDiagonals[i].OccupiedBy;
                if (piece.pieceColor == colorTurn)
                    checkingBools.diagUR = true;
                else
                {
                    if (piece is Bishop || piece is Queen || (piece is Pawn && i == 0))
                        return true;

                    checkingBools.diagUR = true;
                }
            }

            if (checkingBools.diagDL is false && diag.downLeftDiagonals.Count > i && diag.downLeftDiagonals[i].IsOccupied)
            {
                Piece piece = diag.downLeftDiagonals[i].OccupiedBy;
                if (piece.pieceColor == colorTurn)
                    checkingBools.diagDL = true;
                else
                {
                    if (piece is Bishop || piece is Queen || (piece is Pawn && i == 0))
                        return true;

                    checkingBools.diagDL = true;
                }
            }

            if (checkingBools.diagDR is false && diag.downRightDiagonals.Count > i && diag.downRightDiagonals[i].IsOccupied)
            {
                Piece piece = diag.downRightDiagonals[i].OccupiedBy;
                if (piece.pieceColor == colorTurn)
                    checkingBools.diagDR = true;
                else
                {
                    if (piece is Bishop || piece is Queen || (piece is Pawn && i == 0))
                        return true;

                    checkingBools.diagDR = true;
                }
            }
        }

        Knight knight = new(env);
        knight.SetTile(kingTile);
        knight.pieceColor = colorTurn;

        return knight.GetMoves().Any(m => m.to.OccupiedBy is Knight);
    }

    

    struct MoveChecking
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
