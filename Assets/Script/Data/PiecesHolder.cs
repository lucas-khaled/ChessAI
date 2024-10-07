using System;
using System.Collections.Generic;

public class PiecesHolder
{
    public List<Piece> pieces;
    public List<Piece> whitePieces;
    public List<Piece> blackPieces;
    public List<Pawn> whitePawns;
    public List<Pawn> blackPawns;
    public List<Rook> whiteRooks;
    public List<Rook> blackRooks;
    public List<Queen> whiteQueens;
    public List<Queen> blackQueens;
    public List<Bishop> whiteBishops;
    public List<Bishop> blackBishops;
    public List<Knight> whiteKnights;
    public List<Knight> blackKnights;
    public King whiteKing;
    public King blackKing;

    public PiecesHolder() 
    {
        pieces = new();
        whitePieces = new();
        blackPieces = new();
        whitePawns = new();
        blackPawns = new();
        whiteRooks = new();
        blackRooks = new();
        whiteQueens = new();
        blackQueens = new();
        whiteBishops = new();
        blackBishops = new();
        whiteKnights = new();
        blackKnights = new();
    }

    public void AddPiece(Piece piece) 
    {
        pieces.Add(piece);
        PieceColor color = piece.pieceColor;

        if (color == PieceColor.White)
            whitePieces.Add(piece);
        else
            blackPieces.Add(piece);

        if(piece is King king) 
        {
            if (color == PieceColor.Black)
                blackKing = king;
            else
                whiteKing = king;
        }
        else if(piece is Queen queen) 
        {
            if (color == PieceColor.Black)
                blackQueens.Add(queen);
            else
                whiteQueens.Add(queen);
        }
        else if (piece is Rook rook)
        {
            if (color == PieceColor.Black)
                blackRooks.Add(rook);
            else
                whiteRooks.Add(rook);
        }
        else if (piece is Bishop bishop)
        {
            if (color == PieceColor.Black)
                blackBishops.Add(bishop);
            else
                whiteBishops.Add(bishop);
        }
        else if (piece is Knight knight)
        {
            if (color == PieceColor.Black)
                blackKnights.Add(knight);
            else
                whiteKnights.Add(knight);
        }
        else if (piece is Pawn pawn)
        {
            if (color == PieceColor.Black)
                blackPawns.Add(pawn);
            else
                whitePawns.Add(pawn);
        }
    }

    public void RemovePiece(Piece piece) 
    {
        Predicate<Piece> pred = (p) => p.name == piece.name;
        pieces.RemoveAll(pred);
        PieceColor color = piece.pieceColor;

        if (color == PieceColor.White)
            whitePieces.RemoveAll(pred);
        else
            blackPieces.RemoveAll(pred);

        if (piece is King king)
        {
            if (color == PieceColor.Black)
                blackKing = null;
            else
                whiteKing = null;
        }
        else if (piece is Queen queen)
        {
            if (color == PieceColor.Black)
                blackQueens.RemoveAll(pred);
            else
                whiteQueens.RemoveAll(pred);
        }
        else if (piece is Rook rook)
        {
            if (color == PieceColor.Black)
                blackRooks.RemoveAll(pred);
            else
                whiteRooks.RemoveAll(pred);
        }
        else if (piece is Bishop bishop)
        {
            if (color == PieceColor.Black)
                blackBishops.RemoveAll(pred);
            else
                whiteBishops.RemoveAll(pred);
        }
        else if (piece is Knight knight)
        {
            if (color == PieceColor.Black)
                blackKnights.RemoveAll(pred);
            else
                whiteKnights.RemoveAll(pred);
        }
        else if (piece is Pawn pawn)
        {
            if (color == PieceColor.Black)
                blackPawns.RemoveAll(pred);
            else
                whitePawns.RemoveAll(pred);
        }
    }
}
