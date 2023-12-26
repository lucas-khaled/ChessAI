using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesSetup : MonoBehaviour
{
    [SerializeField]
    private PiecesConfig config;

    private static List<Piece> pieces = new();

    public void SetupByFEN(string FEN) 
    {
        string[] fieldsSplited = FEN.Split(' ');
        SetPiecesPosition(fieldsSplited[0]);
        SetInitialColor(fieldsSplited[1]);

        if(fieldsSplited.Length > 2)
            SetCaslling(fieldsSplited[2]);
    }

    private void SetPiecesPosition(string piecesField) 
    {
        string[] piecesSplited = piecesField.Split('/');
        int row = 7;
        int column = 0;

        foreach (var rowString in piecesSplited)
        {
            foreach (var entry in rowString)
            {
                if (char.IsNumber(entry))
                {
                    column += entry - '0';
                    continue;
                }

                PieceColor color = char.IsUpper(entry) ? PieceColor.White : PieceColor.Black;
                var tile = GameManager.Board.GetTiles()[row][column];
                CreatePieceFromEntry(entry, tile, color);

                column++;
            }

            column = 0;
            row--;
        }
    }

    private void CreatePieceFromEntry(char entry, Tile tile, PieceColor color) 
    {
        switch (char.ToLower(entry))
        {
            case 'p':
                InstantiatePiece<Pawn>(tile, color);
                break;
            case 'k':
                InstantiatePiece<King>(tile, color);
                break;
            case 'n':
                InstantiatePiece<Knight>(tile, color);
                break;
            case 'q':
                InstantiatePiece<Queen>(tile, color);
                break;
            case 'r':
                InstantiatePiece<Rook>(tile, color);
                break;
            case 'b':
                InstantiatePiece<Bishop>(tile, color);
                break;
        }
    }

    private void SetInitialColor(string colorField)
    {
        PieceColor color = (colorField == "w") ? PieceColor.White : PieceColor.Black;
        GameManager.TurnManager.ActualTurn = color;
    }

    private void SetCaslling(string castlingString) 
    {
        if (castlingString == "-")
        {
            GameManager.Rules.SetCastle(PieceColor.White);
            GameManager.Rules.SetCastle(PieceColor.Black);
            return;
        }

        if (castlingString.Contains("K") is false)
            GameManager.Rules.SetCastleKingSide(PieceColor.White);

        if (castlingString.Contains("Q") is false)
            GameManager.Rules.SetCastleQueenSide(PieceColor.White);

        if (castlingString.Contains("k") is false)
            GameManager.Rules.SetCastleKingSide(PieceColor.Black);

        if (castlingString.Contains("q") is false)
            GameManager.Rules.SetCastleQueenSide(PieceColor.Black);
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

    public void InstantiatePiece<TPiece>(Tile tile, PieceColor color) where TPiece : Piece
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
