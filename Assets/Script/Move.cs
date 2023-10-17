using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move 
{
    public Tile from;
    public Tile to;
    public Piece capture;

    public Move(Tile from, Tile to, Piece capture = null)
    {
        this.from = from;
        this.to = to;
        this.capture = capture;
    }

    public Move VirtualizeTo(Board board) 
    {
        var fromRow = from.TilePosition.row;
        var fromColumn = from.TilePosition.column;

        var toRow = to.TilePosition.row;
        var toColumn = to.TilePosition.column;
        return new Move(board.GetTiles()[fromRow][fromColumn], board.GetTiles()[toRow][toColumn], capture);
    }
}
