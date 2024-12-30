using System.Collections.Generic;
using UnityEngine;

public class Bitboard
{
    public ulong value;
    
    public Bitboard() 
    {
        value = 0L;
    }

    public Bitboard(ulong value)
    {
        this.value = value;
    }

    public Bitboard(int index)
    {
        index = Mathf.Clamp(index, 0, 63);
        this.value = 1ul << index;
    }

    public void Add(Bitboard bitboard) 
    {
        Add(bitboard.value);
    }

    public void Add(ulong value) 
    {
        this.value = this.value | value;
    }

    public void Remove(Bitboard bitboard) 
    {
        Remove(bitboard.value);
    }

    public void Remove(ulong value) 
    {
        this.value = ~(~this.value | value);
    }

    public void Clear() 
    {
        this.value = 0L;
    }

    public List<int> ConvertToIndexes()
    {
        string binary = value.ConvertToBinaryString();
        List<int> indexes = new List<int>();
        int lenght = binary.Length;

        while (true)
        {
            int index = binary.IndexOf('1');
            if (index <= -1) break;

            indexes.Add(lenght - index - 1);
            binary = binary.Remove(index, 1);
            binary = binary.Insert(index, "0");
        }

        return indexes;
    }

    public string ToVisualString() 
    {
        string final = string.Empty;
        string rowString = string.Empty;
        for(int i = 0; i <= 63; i++) 
        {
            var bitBoard = new Bitboard(i);
            string tile = (this & bitBoard) > 0 ? "x|" : " |";
            
            int mod = i % 8;
            if (mod == 0)
                tile = "|" + tile;

            rowString += tile;

            if (mod == 7) 
            {
                final = rowString + "\n" + final;
                rowString = string.Empty;
            }
        }

        final = "\n" + final;
        final += "\n";
        return final;
    }

    public override string ToString()
    {
        return ToVisualString();
    }

    public static Bitboard operator &(Bitboard a, Bitboard b) => new Bitboard(a.value & b.value);
    public static Bitboard operator |(Bitboard a, Bitboard b) => new Bitboard(a.value | b.value);
    public static bool operator >(Bitboard a, Bitboard b) => a.value > b.value;
    public static bool operator <(Bitboard a, Bitboard b) => a.value < b.value;
    public static bool operator >=(Bitboard a, Bitboard b) => a.value >= b.value;
    public static bool operator <=(Bitboard a, Bitboard b) => a.value <= b.value;
    public static bool operator ==(Bitboard a, Bitboard b) => a.value == b.value;
    public static bool operator !=(Bitboard a, Bitboard b) => a.value != b.value;

    public static bool operator >(Bitboard a, float b) => a.value > b;
    public static bool operator <(Bitboard a, float b) => a.value < b;

    public static bool operator >=(Bitboard a, float b) => a.value >= b;
    public static bool operator <=(Bitboard a, float b) => a.value <= b;

    public static bool operator ==(Bitboard a, float b) => a.value == b;
    public static bool operator !=(Bitboard a, float b) => a.value != b;
}
