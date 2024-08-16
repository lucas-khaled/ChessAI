using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PiecesSetup : ManagerHelper
{
    [SerializeField]
    private PiecesConfig config;

    private List<Piece> pieces = new();
    private Environment environment => manager.environment;

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
        InstantiatePiece<King>(environment.board.GetTiles()[0][4], PieceColor.White);
        InstantiatePiece<King>(environment.board.GetTiles()[7][4], PieceColor.Black);
    }

    private void SetQueens()
    {
        InstantiatePiece<Queen>(environment.board.GetTiles()[0][3], PieceColor.White);
        InstantiatePiece<Queen>(environment.board.GetTiles()[7][3], PieceColor.Black);
    }

    private void SetRooks()
    {
        InstantiatePiece<Rook>(environment.board.GetTiles()[0][7], PieceColor.White);
        InstantiatePiece<Rook>(environment.board.GetTiles()[0][0], PieceColor.White);
        InstantiatePiece<Rook>(environment.board.GetTiles()[7][0], PieceColor.Black);
        InstantiatePiece<Rook>(environment.board.GetTiles()[7][7], PieceColor.Black);
    }

    private void SetBishops()
    {
        InstantiatePiece<Bishop>(environment.board.GetTiles()[0][5], PieceColor.White);
        InstantiatePiece<Bishop>(environment.board.GetTiles()[0][2], PieceColor.White);
        InstantiatePiece<Bishop>(environment.board.GetTiles()[7][5], PieceColor.Black);
        InstantiatePiece<Bishop>(environment.board.GetTiles()[7][2], PieceColor.Black);
    }

    private void SetKnights()
    {
        InstantiatePiece<Knight>(environment.board.GetTiles()[0][6], PieceColor.White);
        InstantiatePiece<Knight>(environment.board.GetTiles()[0][1], PieceColor.White);
        InstantiatePiece<Knight>(environment.board.GetTiles()[7][6], PieceColor.Black);
        InstantiatePiece<Knight>(environment.board.GetTiles()[7][1], PieceColor.Black);
    }

    private void SetPawns()
    {
        for (int i = 0; i<8; i++) 
        {
            InstantiatePiece<Pawn>(environment.board.GetTiles()[1][i], PieceColor.White);
            InstantiatePiece<Pawn>(environment.board.GetTiles()[6][i], PieceColor.Black);
        }
    }

    public void InstantiatePiece(Tile tile, PieceColor color, Type type)
    {
        Piece piece = Activator.CreateInstance(type, environment) as Piece;

        piece.SetTile(tile);
        piece.pieceColor = color;
        pieces.Add(piece);
        tile.Occupy(piece);

        AddVisual(piece, name);

        AddPieceToList(piece);
    }

    public void InstantiatePiece<TPiece>(Tile tile, PieceColor color) where TPiece : Piece
    {
        TPiece piece = Activator.CreateInstance(typeof(TPiece), environment) as TPiece;
      
        piece.SetTile(tile);
        piece.pieceColor = color;  
        pieces.Add(piece);
        tile.Occupy(piece);

        AddVisual(piece, name);

        AddPieceToList(piece);
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

    private void AddPieceToList(Piece piece) 
    {
        environment.board.pieces.Add(piece);

        if (piece.pieceColor == PieceColor.White)
            environment.board.whitePieces.Add(piece);
        else
            environment.board.blackPieces.Add(piece);
    }
}
