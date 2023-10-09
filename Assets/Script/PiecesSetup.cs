using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesSetup : MonoBehaviour
{
    [SerializeField]
    private PiecesConfig config;

    private static List<Piece> pieces = new();

    public static King GetKing(PieceColor color) 
    {
        return pieces.Find(p => p.pieceColor == color && p is King) as King;
    }

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
        InstantiatePiece(GameManager.Board.GetTiles()[0][4], config.kingPrefab, "WKing", PieceColor.White);
        InstantiatePiece(GameManager.Board.GetTiles()[7][4], config.kingPrefab, "BKing", PieceColor.Black);
    }

    private void SetQueens()
    {
        InstantiatePiece(GameManager.Board.GetTiles()[0][3], config.queenPrefab, "WQueen", PieceColor.White);
        InstantiatePiece(GameManager.Board.GetTiles()[7][3], config.queenPrefab, "BQueen", PieceColor.Black);
    }

    private void SetRooks()
    {
        InstantiatePiece(GameManager.Board.GetTiles()[0][7], config.rookPrefab, "WRookR", PieceColor.White);
        InstantiatePiece(GameManager.Board.GetTiles()[0][0], config.rookPrefab, "WRookL", PieceColor.White);
        InstantiatePiece(GameManager.Board.GetTiles()[7][0], config.rookPrefab, "BRookR", PieceColor.Black);
        InstantiatePiece(GameManager.Board.GetTiles()[7][7], config.rookPrefab, "BRookL", PieceColor.Black);
    }

    private void SetBishops()
    {
        InstantiatePiece(GameManager.Board.GetTiles()[0][5], config.bishopPrefab, "WBishopR", PieceColor.White);
        InstantiatePiece(GameManager.Board.GetTiles()[0][2], config.bishopPrefab, "WBishopL", PieceColor.White);
        InstantiatePiece(GameManager.Board.GetTiles()[7][5], config.bishopPrefab, "BBishopR", PieceColor.Black);
        InstantiatePiece(GameManager.Board.GetTiles()[7][2], config.bishopPrefab, "BBishopL", PieceColor.Black);
    }

    private void SetKnights()
    {
        InstantiatePiece(GameManager.Board.GetTiles()[0][6], config.knightPrefab, "WKnightR", PieceColor.White);
        InstantiatePiece(GameManager.Board.GetTiles()[0][1], config.knightPrefab, "WKnightL", PieceColor.White);
        InstantiatePiece(GameManager.Board.GetTiles()[7][6], config.knightPrefab, "BKnightR", PieceColor.Black);
        InstantiatePiece(GameManager.Board.GetTiles()[7][1], config.knightPrefab, "BKnightL", PieceColor.Black);
    }

    private void SetPawns()
    {
        for(int i = 0; i<8; i++) 
        {
            InstantiatePiece(GameManager.Board.GetTiles()[1][i], config.pawnPrefab, "WPawn" + (i + 1), PieceColor.White);
            InstantiatePiece(GameManager.Board.GetTiles()[6][i], config.pawnPrefab, "BPawn" + (i + 1), PieceColor.Black);
        }
    }

    private void InstantiatePiece(Tile tile, Piece prefab, string name, PieceColor color) 
    {
        var piece = Instantiate(prefab);
        piece.name = name;
        piece.MoveTo(tile);
        piece.pieceColor = color;
        piece.GetComponent<Renderer>().material = (color == PieceColor.White) ? config.lightMaterial : config.darkMaterial;

        pieces.Add(piece);
    }
}
