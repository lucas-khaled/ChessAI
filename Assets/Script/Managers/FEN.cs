using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FEN
{
    public delegate void InstantiateCallback(Tile tile, PieceColor color, Type pieceType);

    private Dictionary<char, int> letterColumnToIndex = new Dictionary<char, int>()
    {
        { 'a', 0 },
        { 'b', 1 },
        { 'c', 2 },
        { 'd', 3 },
        { 'e', 4 },
        { 'f', 5 },
        { 'g', 6 },
        { 'h', 7 }
    };

    private Dictionary<char, Type> letterPieceToType = new Dictionary<char, Type>()
    {
        { 'k', typeof(King) },
        { 'n', typeof(Knight) },
        { 'r', typeof(Rook) },
        { 'b', typeof(Bishop) },
        { 'p', typeof(Pawn) },
        { 'q', typeof(Queen) }
    };

    public void SetupByFEN(string FEN, InstantiateCallback instantiateCallback)
    {
        string[] fieldsSplited = FEN.Split(' ');
        SetPiecesPosition(fieldsSplited[0], instantiateCallback);
        SetInitialColor(fieldsSplited[1]);

        if (fieldsSplited.Length > 2)
            SetCaslling(fieldsSplited[2]);

        if (fieldsSplited.Length > 3)
            SetEnPassant(fieldsSplited[3]);

        if (fieldsSplited.Length > 4)
            SetHalfMoves(fieldsSplited[4]);

        if (fieldsSplited.Length > 5)
            SetFullMoves(fieldsSplited[5]);
    }

    private void SetFullMoves(string fullMovesString)
    {
        GameManager.TurnManager.fullMoves = Convert.ToInt32(fullMovesString);
    }

    private void SetHalfMoves(string halfMovesString)
    {
        GameManager.TurnManager.halfMoves = Convert.ToInt32(halfMovesString);
    }

    private void SetPiecesPosition(string piecesField, InstantiateCallback instantiateCallback)
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
                CreatePieceFromEntry(entry, tile, color, instantiateCallback);

                column++;
            }

            column = 0;
            row--;
        }
    }

    private void CreatePieceFromEntry(char entry, Tile tile, PieceColor color, InstantiateCallback instantiateCallback)
    {
        instantiateCallback.Invoke(tile, color, letterPieceToType[char.ToLower(entry)]);
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

    private void SetEnPassant(string enPassantString)
    {
        if (enPassantString == "-") return;

        var column = enPassantString[0];
        var row = enPassantString[1];

        int columnIndex = letterColumnToIndex[column];
        int rowIndex = Convert.ToInt32(row.ToString()) - 1;

        var tile = GameManager.Board.GetTiles()[rowIndex][columnIndex];
        var offset = (GameManager.TurnManager.ActualTurn == PieceColor.White) ? -1 : 1;
        var pawn = GameManager.Board.GetTiles()[rowIndex + offset][columnIndex].OccupiedBy as Pawn;

        GameManager.Rules.SetEnPassant(tile, pawn);
    }

    public string GetFENFrom(Environment enviroment) 
    {
        return GetFENPositions(enviroment.board) 
            + " " + GetFENActiveColor(enviroment.turnManager) 
            + " " + GetFENCastlingRights(enviroment.rules);
    }

    private string GetFENPositions(Board board) 
    {
        var postionsString = "";
        var tiles = board.GetTiles();

        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            int emptyColumnCount = 0;

            for (int j = 0; j < tiles[i].Count; j++)
            {
                var tile = tiles[i][j];

                if (tile.IsOccupied is false) 
                {
                    emptyColumnCount++;
                    continue;
                }

                if(emptyColumnCount > 0)
                    postionsString += emptyColumnCount;
                
                emptyColumnCount = 0;

                postionsString += GetPieceChar(tile.OccupiedBy);
            }

            if (emptyColumnCount > 0)
                postionsString += emptyColumnCount;

            if(i > 0)
                postionsString += "/";
        }

        return postionsString;
    }

    private char GetPieceChar(Piece piece) 
    {
        var pieceChar = letterPieceToType.FirstOrDefault(x => x.Value == piece.GetType()).Key;

        return (piece.pieceColor == PieceColor.White) ?
             char.ToUpper(pieceChar) :
             char.ToLower(pieceChar);
    }

    private string GetFENActiveColor(TurnManager turnManager)
    {
        return turnManager.ActualTurn == PieceColor.White ? "w" : "b";
    }

    private string GetFENCastlingRights(EspecialRules rules)
    {
        string returnString = string.Empty;
        if (rules.whiteCanCastleKingSide)
            returnString += "K";

        if (rules.whiteCanCastleQueenSide)
            returnString += "Q";

        if (rules.blackCanCastleKingSide)
            returnString += "k";

        if (rules.blackCanCastleQueenSide)
            returnString += "q";

        return returnString;
    }
}
