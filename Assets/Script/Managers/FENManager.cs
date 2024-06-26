using System;
using System.Collections.Generic;
using System.Linq;

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

    private Environment environment;
    public FENManager(Environment environment) 
    {
        this.environment = environment;
    }

    public void SetupByFEN(FEN fen, InstantiateCallback instantiateCallback)
    {
        SetPiecesPosition(fen.positions, instantiateCallback);
        SetInitialColor(fen.pieceColor);
        SetCaslling(fen.castlingString);
        SetEnPassant(fen.enPassantString);
        SetHalfMoves(fen.halfMovesString);
        SetFullMoves(fen.fullMovesString);
    }

    private void SetFullMoves(string fullMovesString)
    {
        environment.turnManager.fullMoves = Convert.ToInt32(fullMovesString) - 1;
    }

    private void SetHalfMoves(string halfMovesString)
    {
        environment.turnManager.halfMoves = Convert.ToInt32(halfMovesString);
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
                var tile = environment.board.GetTiles()[row][column];
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
        environment.turnManager.ActualTurn = color;
    }

    private void SetCaslling(string castlingString)
    {
        if (castlingString == "-")
        {
            environment.rules.SetCastle(PieceColor.White);
            environment.rules.SetCastle(PieceColor.Black);
            return;
        }

        if (castlingString.Contains("K") is false)
            environment.rules.SetCastleKingSide(PieceColor.White);

        if (castlingString.Contains("Q") is false)
            environment.rules.SetCastleQueenSide(PieceColor.White);

        if (castlingString.Contains("k") is false)
            environment.rules.SetCastleKingSide(PieceColor.Black);

        if (castlingString.Contains("q") is false)
            environment.rules.SetCastleQueenSide(PieceColor.Black);
    }

    private void SetEnPassant(string enPassantString)
    {
        if (enPassantString == "-") return;

        var column = enPassantString[0];
        var row = enPassantString[1];

        int columnIndex = letterColumnToIndex[column];
        int rowIndex = Convert.ToInt32(row.ToString()) - 1;

        var tile = environment.board.GetTiles()[rowIndex][columnIndex];
        var offset = (environment.turnManager.ActualTurn == PieceColor.White) ? -1 : 1;
        var pawn = environment.board.GetTiles()[rowIndex + offset][columnIndex].OccupiedBy as Pawn;

        environment.rules.SetEnPassant(tile, pawn);
    }

    public FEN GetFEN() 
    {
        string fullString = GetFENPositions(environment.board) 
            + " " + GetFENActiveColor(environment.turnManager) 
            + " " + GetFENCastlingRights(environment.rules)
            + " " + GetFENEnPassant(environment.rules)
            + " " + environment.turnManager.halfMoves
            + " " + (environment.turnManager.fullMoves + 1);

        return new FEN(fullString);
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
