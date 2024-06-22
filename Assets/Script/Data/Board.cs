using System.Collections.Generic;
using System.Linq;

public class Board : IEnvironmentable
{
    public int BoardRowSize;
    public int BoardColumnSize;

    public List<List<Tile>> tiles;
    public List<Piece> pieces;

    public Environment Environment { get; }

    public Board(int row, int column, Environment environment)
    {
        BoardRowSize = row;
        BoardColumnSize = column;
        tiles = new();
        pieces = new();
        Environment = environment;
    }

    public IEnvironmentable Copy(Environment env)
    {
        List<List<Tile>> virtualTiles = new List<List<Tile>>();
        List<Piece> pieces = new List<Piece>();

        foreach (var list in tiles)
        {
            List<Tile> virtualList = new();

            foreach (var tile in list)
            {
                var copyTile = tile.Copy(env) as Tile;
                virtualList.Add(copyTile);
                if (tile.IsOccupied)
                    pieces.Add(copyTile.OccupiedBy);
            }

            virtualTiles.Add(virtualList);
        }

        return new Board(BoardRowSize, BoardColumnSize, env)
        {
            tiles = virtualTiles,
            pieces = pieces,
            BoardColumnSize = this.BoardColumnSize,
            BoardRowSize = this.BoardRowSize
        };
    }

    public List<List<Tile>> GetTiles()
    {
        return tiles;
    }
}