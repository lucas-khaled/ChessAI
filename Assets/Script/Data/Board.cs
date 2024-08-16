using System.Collections.Generic;
using System.Linq;

public class Board : IEnvironmentable
{
    public int BoardRowSize;
    public int BoardColumnSize;

    public List<List<Tile>> tiles;
    public List<Piece> pieces;
    public List<Piece> whitePieces;
    public List<Piece> blackPieces;

    public Environment Environment { get; }

    public Board(int row, int column, Environment environment)
    {
        BoardRowSize = row;
        BoardColumnSize = column;
        tiles = new();
        pieces = new();
        whitePieces = new();
        blackPieces = new();
        Environment = environment;
    }

    public IEnvironmentable Copy(Environment env)
    {
        List<List<Tile>> virtualTiles = new List<List<Tile>>();
        List<Piece> pieces = new List<Piece>();
        List<Piece> whitePieces = new List<Piece>();
        List<Piece> blackPieces = new List<Piece>();

        foreach (var list in tiles)
        {
            List<Tile> virtualList = new();

            foreach (var tile in list)
            {
                var copyTile = tile.Copy(env) as Tile;
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

        return new Board(BoardRowSize, BoardColumnSize, env)
        {
            tiles = virtualTiles,
            pieces = pieces,
            whitePieces = whitePieces,
            blackPieces = blackPieces,
            BoardColumnSize = this.BoardColumnSize,
            BoardRowSize = this.BoardRowSize
        };
    }

    public List<List<Tile>> GetTiles()
    {
        return tiles;
    }
}