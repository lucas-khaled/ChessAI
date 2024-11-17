using UnityEngine;

public class Bitboard
{
    public long value;
    
    public Bitboard() 
    {
        value = 0L;
    }

    public Bitboard(long value)
    {
        this.value = value;
    }

    public Bitboard(int index)
    {
        index = Mathf.Clamp(index, 0, 63);
        this.value = 1L << index;
    }

    public void Add(Bitboard bitboard) 
    {
        Add(bitboard.value);
    }

    public void Add(long value) 
    {
        this.value = this.value | value;
    }

    public void Remove(long value) 
    {
        this.value = ~(~this.value | value);
    }
}
