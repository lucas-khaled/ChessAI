using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Move 
{
    public Tile from;
    public Tile to;

    public Move(Tile from, Tile to)
    {
        this.from = from;
        this.to = to;
    }

    public Move VirtualizeTo(Board board) 
    {
        var fromRow = from.TilePosition.row;
        var fromColumn = from.TilePosition.column;

        var toRow = to.TilePosition.row;
        var toColumn = to.TilePosition.column;
        return new Move()
        {
            from = board.GetTiles()[fromRow][fromColumn],
            to = board.GetTiles()[toRow][toColumn]
        };
    }
}
