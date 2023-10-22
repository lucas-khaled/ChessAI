using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TurnManager
{
    public static PieceColor ActualTurn { get; private set; } = PieceColor.White;

    private static List<Move> moves = new List<Move>();

    public static Action<PieceColor> onTurnMade;

    private static MoveMaker moveMaker = new();

    public static void SetMove(Move move) 
    {
        moveMaker.DoMove(move);

        onTurnMade?.Invoke(ActualTurn);

        moves.Add(move);
        ActualTurn = (ActualTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;
    }

    public static Move[] GetLegalMoves(Move[] moves) 
    {
       var returningMoves = FilterCheckMoves(moves);

        return returningMoves.ToArray();
    }

    private static List<Move> FilterCheckMoves(Move[] moves)
    {
        List<Move> validMoves = new List<Move>();
        foreach (var move in moves) 
        {
            Board virtualBoard = GameManager.Board.Copy();

            var virtualMove = move.VirtualizeTo(virtualBoard);
            moveMaker.DoMove(virtualMove, true);

            if(IsCheck(virtualBoard, ActualTurn) is false) 
                validMoves.Add(move);
        }

        return validMoves;
    }

    public static bool IsCheck(Board board, PieceColor colorTurn) 
    {
        var manager = GameManager.BoardManager;

        Tile kingTile = board.GetKingTile(colorTurn);
        Verticals vert = manager.GetVerticalsFrom(board, kingTile.TilePosition, colorTurn);
        Horizontals hor = manager.GetHorizontalsFrom(board, kingTile.TilePosition, colorTurn);
        Diagonals diag = manager.GetDiagonalsFrom(board, kingTile.TilePosition, colorTurn);

        MoveChecking checkingBools = new();

        for(int i = 0; i < 8; i++) 
        {
            if(checkingBools.vertD is false && vert.backVerticals.Count>i && vert.backVerticals[i].IsOccupied)
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

        Knight knight = new();
        knight.SetTile(kingTile, true);
        knight.pieceColor = colorTurn;

        return knight.GetMoves(board).Any(m => m.to.OccupiedBy is Knight);
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
