using System.Collections.Generic;

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

        foreach (var list in tiles)
        {
            List<Tile> virtualList = new();

            foreach (var tile in list)
            {
                virtualList.Add(tile.Copy(env) as Tile);
            }

            virtualTiles.Add(virtualList);
        }

        return new Board(BoardRowSize, BoardColumnSize, env)
        {
            tiles = virtualTiles,
            pieces = new(pieces),
            BoardColumnSize = this.BoardColumnSize,
            BoardRowSize = this.BoardRowSize
        };
    }

    public List<List<Tile>> GetTiles()
    {
        return tiles;
    }
}