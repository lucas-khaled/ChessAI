using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Move 
{
    public Tile from;
    public Tile to;
    public Piece capture;

    public Move(Tile from, Tile to)
    {
        this.from = from;
        this.to = to;
        capture = null;
    }

    public Move(Tile from, Tile to, Piece capture) : this(from, to)
    {
        this.capture = capture;
    }
}
