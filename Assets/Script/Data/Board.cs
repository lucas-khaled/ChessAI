using System.Collections.Generic;
using System.Linq;

public class Board
{
    public int BoardRowSize;
    public int BoardColumnSize;

    public List<List<Tile>> tiles;
    public List<Piece> pieces;
    public List<Piece> whitePieces;
    public List<Piece> blackPieces;

    public EspecialRules rules;
    public BoardEvents events;

    public string ActualHash;

    public List<Turn> turns { get; private set; } = new List<Turn>();
    public Turn LastTurn => turns.Count > 0 ? turns[turns.Count - 1] : new Turn();
    public PieceColor ActualTurn { get; set; } = PieceColor.White;
    public FENManager FENManager { get; private set; }

    public string Name { get; set; }

    public Board(int row, int column)
    {
        BoardRowSize = row;
        BoardColumnSize = column;
        tiles = new();
        pieces = new();
        whitePieces = new();
        blackPieces = new();
        events = new BoardEvents();
        rules = new EspecialRules(this);
        FENManager = new FENManager(this);
    }

    public Board Copy()
    {
        List<List<Tile>> virtualTiles = new List<List<Tile>>();
        List<Piece> pieces = new List<Piece>();
        List<Piece> whitePieces = new List<Piece>();
        List<Piece> blackPieces = new List<Piece>();
        Board board = new Board(BoardRowSize, BoardColumnSize);
        board.Name = "Copy board";

        foreach (var list in tiles)
        {
            List<Tile> virtualList = new();

            foreach (var tile in list)
            {
                var copyTile = tile.Copy(board);
                virtualList.Add(copyTile);

                if (tile.IsOccupied)
                {
                    Piece piece = copyTile.OccupiedBy;
                    pieces.Add(piece);

                    if (piece.pieceColor == PieceColor.White)
                        whitePieces.Add(piece);
                    else
                        blackPieces.Add(piece);
                }
            }

            virtualTiles.Add(virtualList);
        }

        board.tiles = virtualTiles;
        board.pieces = pieces;
        board.whitePieces = whitePieces;
        board.blackPieces = blackPieces;
        board.rules = rules.Copy(board);
        return board;
    }

    public List<List<Tile>> GetTiles()
    {
        return tiles;
    }

    public Tile GetKingTile(PieceColor color)
    {
        foreach (var row in tiles)
        {
            var kingTile = row.Find(t => t.OccupiedBy is King king && king.pieceColor == color);
            if (kingTile != null)
                return kingTile;
        }

        return null;
    }

    public Tile[] GetRookTiles(PieceColor color)
    {
        List<Tile> rookTiles = new();
        foreach (var row in this.tiles)
        {
            var rookTile = row.Where(t => t.OccupiedBy is Rook rook && rook.pieceColor == color);
            if (rookTile != null && rookTile.ToList().Count > 0)
            {
                rookTiles.AddRange(rookTile);
                if (rookTiles.Count >= 2) break;
            }
        }

        return rookTiles.ToArray();
    }

    public Piece[] GetAllPieces(PieceColor pieceColor)
    {
        List<Piece> pieces = new();
        foreach (var tileList in tiles)
        {
            foreach (var tile in tileList)
            {
                if (tile.IsOccupied && tile.OccupiedBy.pieceColor == pieceColor)
                    pieces.Add(tile.OccupiedBy);
            }
        }

        return pieces.ToArray();
    }

    public void Clear()
    {
        foreach (var row in tiles)
        {
            foreach (var tile in row)
            {
                tile.DeOccupy();
            }
        }
    }

    public override string ToString()
    {
        return Name;
    }
}