using System.Collections.Generic;

public struct Board
{
    public int BoardRowSize;
    public int BoardColumnSize;

    public List<List<Tile>> tiles;

    public Board(int row, int column)
    {
        BoardRowSize = row;
        BoardColumnSize = column;
        tiles = new List<List<Tile>>();
    }

    public Board Copy()
    {
        List<List<Tile>> virtualTiles = new List<List<Tile>>();

        foreach (var list in tiles)
        {
            List<Tile> virtualList = new();

            foreach (var tile in list)
            {
                virtualList.Add(tile.Copy());
            }

            virtualTiles.Add(virtualList);
        }

        return new Board
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