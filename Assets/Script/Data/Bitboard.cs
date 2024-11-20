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

    public void Remove(ulong value) 
    {
        this.value = ~(~this.value | value);
    }
}
