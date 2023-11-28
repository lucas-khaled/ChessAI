using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesSetup : MonoBehaviour
{
    [SerializeField]
    private PiecesConfig config;

    private static List<Piece> pieces = new();

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
        InstantiatePiece<King>(GameManager.Board.GetTiles()[0][4], "WKing", PieceColor.White);
        InstantiatePiece<King>(GameManager.Board.GetTiles()[7][4], "BKing", PieceColor.Black);
    }

    private void SetQueens()
    {
        InstantiatePiece<Queen>(GameManager.Board.GetTiles()[0][3], "WQueen", PieceColor.White);
        InstantiatePiece<Queen>(GameManager.Board.GetTiles()[7][3], "BQueen", PieceColor.Black);
    }

    private void SetRooks()
    {
        InstantiatePiece<Rook>(GameManager.Board.GetTiles()[0][7], "WRookR", PieceColor.White);
        InstantiatePiece<Rook>(GameManager.Board.GetTiles()[0][0], "WRookL", PieceColor.White);
        InstantiatePiece<Rook>(GameManager.Board.GetTiles()[7][0], "BRookR", PieceColor.Black);
        InstantiatePiece<Rook>(GameManager.Board.GetTiles()[7][7], "BRookL", PieceColor.Black);
    }

    private void SetBishops()
    {
        InstantiatePiece<Bishop>(GameManager.Board.GetTiles()[0][5], "WBishopR", PieceColor.White);
        InstantiatePiece<Bishop>(GameManager.Board.GetTiles()[0][2], "WBishopL", PieceColor.White);
        InstantiatePiece<Bishop>(GameManager.Board.GetTiles()[7][5], "BBishopR", PieceColor.Black);
        InstantiatePiece<Bishop>(GameManager.Board.GetTiles()[7][2], "BBishopL", PieceColor.Black);
    }

    private void SetKnights()
    {
        InstantiatePiece<Knight>(GameManager.Board.GetTiles()[0][6], "WKnightR", PieceColor.White);
        InstantiatePiece<Knight>(GameManager.Board.GetTiles()[0][1], "WKnightL", PieceColor.White);
        InstantiatePiece<Knight>(GameManager.Board.GetTiles()[7][6], "BKnightR", PieceColor.Black);
        InstantiatePiece<Knight>(GameManager.Board.GetTiles()[7][1], "BKnightL", PieceColor.Black);
    }

    private void SetPawns()
    {
        for (int i = 0; i<8; i++) 
        {
            InstantiatePiece<Pawn>(GameManager.Board.GetTiles()[1][i],"WPawn" + (i + 1), PieceColor.White);
            InstantiatePiece<Pawn>(GameManager.Board.GetTiles()[6][i], "BPawn" + (i + 1), PieceColor.Black);
        }
    }

    public void InstantiatePiece<TPiece>(Tile tile, string name, PieceColor color) where TPiece : Piece
    {
        TPiece piece = Activator.CreateInstance(typeof(TPiece), GameManager.environment) as TPiece;
      
        piece.SetTile(tile);
        piece.pieceColor = color;  
        pieces.Add(piece);
        tile.Occupy(piece);

        AddVisual(piece, name);
    }

    public void AddVisual(Piece piece, string name)
    {
        var prefab = config.GetPrefabFromPiece(piece);

        var visualPiece = Instantiate(prefab);
        visualPiece.name = name;
        visualPiece.SetPiece(piece, config);

        piece.visualPiece = visualPiece;
        visualPiece.SetTilePosition(piece.GetTile());
    }
}
