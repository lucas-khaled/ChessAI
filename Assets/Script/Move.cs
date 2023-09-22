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
}
