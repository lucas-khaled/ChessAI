using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FENManager
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

    private Board board;

    public FENManager(Board board) 
    {
        this.board = board;
    }

    public void SetupByFEN(FEN fen, InstantiateCallback instantiateCallback, GameManager manager)
    {
        Turn turn = new Turn();

        SetPiecesPosition(fen.positions, instantiateCallback);
        SetInitialColor(fen.pieceColor);
        SetCastling(fen.castlingString);
        SetEnPassant(fen.enPassantString);

        turn.halfMoves = GetHalfMoves(fen.halfMovesString);
        turn.fullMoves = GetFullMoves(fen.fullMovesString);

        long hash = manager.HashManager.GetHashFromPosition(board);
        board.ActualHash = turn.zobristHash = hash.ToString();

        board.turns.Add(turn);
    }

    private int GetFullMoves(string fullMovesString)
    {
        
        return Convert.ToInt32(fullMovesString) - 1;
    }

    private int GetHalfMoves(string halfMovesString)
    {
        return Convert.ToInt32(halfMovesString);
    }

    private void SetPiecesPosition(string[] piecesSplitted, InstantiateCallback instantiateCallback)
    {
        int row = 7;
        int column = 0;

        foreach (var rowString in piecesSplitted)
        {
            foreach (var entry in rowString)
            {
                if (char.IsNumber(entry))
                {
                    column += entry - '0';
                    continue;
                }

                PieceColor color = char.IsUpper(entry) ? PieceColor.White : PieceColor.Black;
                var tile = board.GetTiles()[row][column];
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

    private void SetInitialColor(PieceColor color)
    {
        board.ActualTurn = color;
    }

    private void SetCastling(string castlingString)
    {
        bool castledWhiteKingside = (castlingString != "-" && castlingString.Contains("K"));
        bool castledWhiteQueenside = (castlingString != "-" && castlingString.Contains("Q"));
        bool castledBlackKingside = (castlingString != "-" && castlingString.Contains("K"));
        bool castledBlackQueenside = (castlingString != "-" && castlingString.Contains("Q"));

        board.rules.SetCastleKingSide(PieceColor.White, castledWhiteKingside, true);
        board.rules.SetCastleQueenSide(PieceColor.White, castledWhiteQueenside, true);
        board.rules.SetCastleKingSide(PieceColor.Black, castledBlackKingside, true);
        board.rules.SetCastleQueenSide(PieceColor.Black, castledBlackQueenside, true);
    }

    private void SetEnPassant(string enPassantString)
    {
        if (enPassantString == "-") return;

        var column = enPassantString[0];
        var row = enPassantString[1];

        int columnIndex = letterColumnToIndex[column];
        int rowIndex = Convert.ToInt32(row.ToString()) - 1;

        var tile = board.GetTiles()[rowIndex][columnIndex];
        var offset = (board.ActualTurn == PieceColor.White) ? -1 : 1;
        var pawn = board.GetTiles()[rowIndex + offset][columnIndex].OccupiedBy as Pawn;

        board.rules.SetEnPassant(tile, pawn);
    }

    public FEN GetFEN() 
    {
        string fullString = GetFENPositions() 
            + " " + GetFENActiveColor() 
            + " " + GetFENCastlingRights(board.rules)
            + " " + GetFENEnPassant(board.rules)
            + " " + board.LastTurn.halfMoves
            + " " + (board.LastTurn.fullMoves + 1);

        return new FEN(fullString);
    }

    private string GetFENPositions() 
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

    private string GetFENActiveColor()
    {
        return board.ActualTurn == PieceColor.White ? "w" : "b";
    }

    private string GetFENCastlingRights(EspecialRules rules)
    {
        string returnString = string.Empty;
        if (rules.whiteCastleRights.CanCastleKingSide) 
            returnString += "K";

        if (rules.whiteCastleRights.CanCastleQueenSide)
            returnString += "Q";

        if (rules.blackCastleRights.CanCastleKingSide)
            returnString += "k";

        if (rules.blackCastleRights.CanCastleQueenSide)
            returnString += "q";

        if (returnString == string.Empty)
            returnString = "-";

        return returnString;
    }

    private string GetFENEnPassant(EspecialRules rules)
    {
        if(rules.enPassantTile == null) return "-";

        var pos = rules.enPassantTile.TilePosition;
        return GetTileStringPosition(pos);
    }

    private string GetTileStringPosition(TileCoordinates coord) 
    {
        var column = letterColumnToIndex.FirstOrDefault(x => x.Value == coord.column).Key;
        var row = coord.row + 1;

        return column + row.ToString();
    }
}
