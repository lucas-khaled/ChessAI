using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static PieceColor GetOppositeColor(this PieceColor color) 
    {
        return (color == PieceColor.White) ? PieceColor.Black : PieceColor.White;
    }
}
