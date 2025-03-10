using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static PieceColor GetOppositeColor(this PieceColor color) 
    {
        return (color == PieceColor.White) ? PieceColor.Black : PieceColor.White;
    }

    public static Bitboard GetBitboard(this List<Move> moves)
    {
        Bitboard bitboard = new Bitboard();

        if(moves == null) return bitboard;

        foreach (var move in moves)
        {
            bitboard.Add(move.to.Bitboard);
        }

        return bitboard;
    }

    public static string ConvertToBinaryString(this ulong value, bool pad = false)
    {
        string binary = Convert.ToString((long)value, 2);

        return (pad) ? binary.PadLeft(64, '0') : binary;
    }
}
