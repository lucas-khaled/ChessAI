using System.Collections.Generic;

public class Board : IEnvironmentable
{
    public int BoardRowSize;
    public int BoardColumnSize;

    public List<List<Tile>> tiles;

    public Environment Environment { get; }

    public Board(int row, int column, Environment environment)
    {
        BoardRowSize = row;
        BoardColumnSize = column;
        tiles = new List<List<Tile>>();
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
            BoardColumnSize = this.BoardColumnSize,
            BoardRowSize = this.BoardRowSize
        };
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
}