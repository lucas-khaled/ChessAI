using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PiecesSetup : ManagerHelper
{
    [SerializeField]
    private PiecesConfig config;

    private List<Piece> pieces = new();
    private Board board => manager.GameBoard;

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
        InstantiatePiece<King>(board.GetTiles()[0][4], PieceColor.White);
        InstantiatePiece<King>(board.GetTiles()[7][4], PieceColor.Black);
    }

    private void SetQueens()
    {
        InstantiatePiece<Queen>(board.GetTiles()[0][3], PieceColor.White);
        InstantiatePiece<Queen>(board.GetTiles()[7][3], PieceColor.Black);
    }

    private void SetRooks()
    {
        InstantiatePiece<Rook>(board.GetTiles()[0][7], PieceColor.White);
        InstantiatePiece<Rook>(board.GetTiles()[0][0], PieceColor.White);
        InstantiatePiece<Rook>(board.GetTiles()[7][0], PieceColor.Black);
        InstantiatePiece<Rook>(board.GetTiles()[7][7], PieceColor.Black);
    }

    private void SetBishops()
    {
        InstantiatePiece<Bishop>(board.GetTiles()[0][5], PieceColor.White);
        InstantiatePiece<Bishop>(board.GetTiles()[0][2], PieceColor.White);
        InstantiatePiece<Bishop>(board.GetTiles()[7][5], PieceColor.Black);
        InstantiatePiece<Bishop>(board.GetTiles()[7][2], PieceColor.Black);
    }

    private void SetKnights()
    {
        InstantiatePiece<Knight>(board.GetTiles()[0][6], PieceColor.White);
        InstantiatePiece<Knight>(board.GetTiles()[0][1], PieceColor.White);
        InstantiatePiece<Knight>(board.GetTiles()[7][6], PieceColor.Black);
        InstantiatePiece<Knight>(board.GetTiles()[7][1], PieceColor.Black);
    }

    private void SetPawns()
    {
        for (int i = 0; i<8; i++) 
        {
            InstantiatePiece<Pawn>(board.GetTiles()[1][i], PieceColor.White);
            InstantiatePiece<Pawn>(board.GetTiles()[6][i], PieceColor.Black);
        }
    }

    public void InstantiatePiece(Tile tile, PieceColor color, Type type)
    {
        Piece piece = Activator.CreateInstance(type, board) as Piece;

        piece.SetTile(tile);
        piece.pieceColor = color;
        pieces.Add(piece);
        tile.Occupy(piece);

        AddVisual(piece);

        AddPieceToList(piece);
    }

    public void InstantiatePiece<TPiece>(Tile tile, PieceColor color) where TPiece : Piece
    {
        TPiece piece = Activator.CreateInstance(typeof(TPiece), board) as TPiece;
      
        piece.SetTile(tile);
        piece.pieceColor = color;  
        pieces.Add(piece);
        tile.Occupy(piece);

        AddVisual(piece);

        AddPieceToList(piece);
    }

    public void AddVisual(Piece piece)
    {
        var prefab = config.GetPrefabFromPiece(piece);

        var visualPiece = Instantiate(prefab);
        visualPiece.SetPiece(piece, config);

        piece.visualPiece = visualPiece;
        visualPiece.SetTilePosition(piece.GetTile());
    }

    private void AddPieceToList(Piece piece) 
    {
        board.pieces.Add(piece);

        if (piece.pieceColor == PieceColor.White)
            board.whitePieces.Add(piece);
        else
            board.blackPieces.Add(piece);
    }
}
