using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesSetup
{
    public void SetInitialPieces() 
    {
        SetKings();
        SetQueens();
        SetRooks();
        SetBishops();
        SetKnights();
        SetPawns();
    }

    private void SetKings()
    {
        var whiteKing = new GameObject("WKing", typeof(King)).GetComponent<King>();
        whiteKing.MoveTo(GameManager.Board.GetTiles()[0][4]);
        whiteKing.pieceColor = PieceColor.White;

        var blackKing = new GameObject("BKing", typeof(King)).GetComponent<King>();
        blackKing.MoveTo(GameManager.Board.GetTiles()[7][4]);
        blackKing.pieceColor = PieceColor.Black;
    }

    private void SetQueens()
    {
        var whiteKing = new GameObject("WQueen", typeof(Queen)).GetComponent<Queen>();
        whiteKing.MoveTo(GameManager.Board.GetTiles()[0][3]);
        whiteKing.pieceColor = PieceColor.White;

        var blackKing = new GameObject("BQueen", typeof(Queen)).GetComponent<Queen>();
        blackKing.MoveTo(GameManager.Board.GetTiles()[7][3]);
        blackKing.pieceColor = PieceColor.Black;
    }

    private void SetRooks()
    {
        var whiteRookRight = new GameObject("WRookR", typeof(Rook)).GetComponent<Rook>();
        whiteRookRight.MoveTo(GameManager.Board.GetTiles()[0][7]);
        whiteRookRight.pieceColor = PieceColor.White;

        var whiteRookLeft = new GameObject("WRookL", typeof(Rook)).GetComponent<Rook>();
        whiteRookLeft.MoveTo(GameManager.Board.GetTiles()[0][0]);
        whiteRookLeft.pieceColor = PieceColor.White;

        var blackRookRight = new GameObject("BRookR", typeof(Rook)).GetComponent<Rook>();
        blackRookRight.MoveTo(GameManager.Board.GetTiles()[7][0]);
        blackRookRight.pieceColor = PieceColor.Black;

        var blackRookLeft = new GameObject("BRookL", typeof(Rook)).GetComponent<Rook>();
        blackRookLeft.MoveTo(GameManager.Board.GetTiles()[7][7]);
        blackRookLeft.pieceColor = PieceColor.Black;
    }

    private void SetBishops()
    {
        var whiteBishopRight = new GameObject("WBishopR", typeof(Bishop)).GetComponent<Bishop>();
        whiteBishopRight.MoveTo(GameManager.Board.GetTiles()[0][5]);
        whiteBishopRight.pieceColor = PieceColor.White;

        var whiteBishopLeft = new GameObject("WBishopL", typeof(Bishop)).GetComponent<Bishop>();
        whiteBishopLeft.MoveTo(GameManager.Board.GetTiles()[0][2]);
        whiteBishopLeft.pieceColor = PieceColor.White;

        var blackBishopRight = new GameObject("BBishopR", typeof(Bishop)).GetComponent<Bishop>();
        blackBishopRight.MoveTo(GameManager.Board.GetTiles()[7][5]);
        blackBishopRight.pieceColor = PieceColor.Black;

        var blackBishopLeft = new GameObject("BBishopL", typeof(Bishop)).GetComponent<Bishop>();
        blackBishopLeft.MoveTo(GameManager.Board.GetTiles()[7][2]);
        blackBishopLeft.pieceColor = PieceColor.Black;
    }

    private void SetKnights()
    {
        var whiteBishopRight = new GameObject("WKnightR", typeof(Knight)).GetComponent<Knight>();
        whiteBishopRight.MoveTo(GameManager.Board.GetTiles()[0][6]);
        whiteBishopRight.pieceColor = PieceColor.White;

        var whiteBishopLeft = new GameObject("WKnightL", typeof(Knight)).GetComponent<Knight>();
        whiteBishopLeft.MoveTo(GameManager.Board.GetTiles()[0][1]);
        whiteBishopLeft.pieceColor = PieceColor.White;

        var blackBishopRight = new GameObject("BKnightR", typeof(Knight)).GetComponent<Knight>();
        blackBishopRight.MoveTo(GameManager.Board.GetTiles()[7][6]);
        blackBishopRight.pieceColor = PieceColor.Black;

        var blackBishopLeft = new GameObject("BKnightL", typeof(Knight)).GetComponent<Knight>();
        blackBishopLeft.MoveTo(GameManager.Board.GetTiles()[7][1]);
        blackBishopLeft.pieceColor = PieceColor.Black;
    }

    private void SetPawns()
    {
        for(int i = 0; i<8; i++) 
        {
            var wPawn = new GameObject("WPawn" + (i + 1), typeof(Pawn)).GetComponent<Pawn>();
            wPawn.MoveTo(GameManager.Board.GetTiles()[1][i]);
            wPawn.pieceColor = PieceColor.White;

            var bPawn = new GameObject("BPawn" + (i + 1), typeof(Pawn)).GetComponent<Pawn>();
            bPawn.MoveTo(GameManager.Board.GetTiles()[6][i]);
            bPawn.pieceColor = PieceColor.Black;
        }
    }
}
