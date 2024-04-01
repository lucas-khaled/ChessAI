using System;
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
        InstantiatePiece<King>(GameManager.Board.GetTiles()[0][4], PieceColor.White);
        InstantiatePiece<King>(GameManager.Board.GetTiles()[7][4], PieceColor.Black);
    }

    private void SetQueens()
    {
        InstantiatePiece<Queen>(GameManager.Board.GetTiles()[0][3], PieceColor.White);
        InstantiatePiece<Queen>(GameManager.Board.GetTiles()[7][3], PieceColor.Black);
    }

    private void SetRooks()
    {
        InstantiatePiece<Rook>(GameManager.Board.GetTiles()[0][7], PieceColor.White);
        InstantiatePiece<Rook>(GameManager.Board.GetTiles()[0][0], PieceColor.White);
        InstantiatePiece<Rook>(GameManager.Board.GetTiles()[7][0], PieceColor.Black);
        InstantiatePiece<Rook>(GameManager.Board.GetTiles()[7][7], PieceColor.Black);
    }

    private void SetBishops()
    {
        InstantiatePiece<Bishop>(GameManager.Board.GetTiles()[0][5], PieceColor.White);
        InstantiatePiece<Bishop>(GameManager.Board.GetTiles()[0][2], PieceColor.White);
        InstantiatePiece<Bishop>(GameManager.Board.GetTiles()[7][5], PieceColor.Black);
        InstantiatePiece<Bishop>(GameManager.Board.GetTiles()[7][2], PieceColor.Black);
    }

    private void SetKnights()
    {
        InstantiatePiece<Knight>(GameManager.Board.GetTiles()[0][6], PieceColor.White);
        InstantiatePiece<Knight>(GameManager.Board.GetTiles()[0][1], PieceColor.White);
        InstantiatePiece<Knight>(GameManager.Board.GetTiles()[7][6], PieceColor.Black);
        InstantiatePiece<Knight>(GameManager.Board.GetTiles()[7][1], PieceColor.Black);
    }

    private void SetPawns()
    {
        for (int i = 0; i<8; i++) 
        {
            InstantiatePiece<Pawn>(GameManager.Board.GetTiles()[1][i], PieceColor.White);
            InstantiatePiece<Pawn>(GameManager.Board.GetTiles()[6][i], PieceColor.Black);
        }
    }

    public void InstantiatePiece(Tile tile, PieceColor color, Type type)
    {
        Piece piece = Activator.CreateInstance(type, GameManager.environment) as Piece;

        piece.SetTile(tile);
        piece.pieceColor = color;
        pieces.Add(piece);
        tile.Occupy(piece);

        AddVisual(piece, name);

        GameManager.Board.pieces.Add(piece);
    }

    public void InstantiatePiece<TPiece>(Tile tile, PieceColor color) where TPiece : Piece
    {
        TPiece piece = Activator.CreateInstance(typeof(TPiece), GameManager.environment) as TPiece;
      
        piece.SetTile(tile);
        piece.pieceColor = color;  
        pieces.Add(piece);
        tile.Occupy(piece);

        AddVisual(piece, name);

        GameManager.Board.pieces.Add(piece);
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
