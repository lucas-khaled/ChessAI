using System.Collections.Generic;

public struct TileCoordinates
{
    public int row;
    public int column;

    private char[] columnNames;

    public TileCoordinates(int row, int column)
    {
        this.row = row;
        this.column = column;

        columnNames = new char[8]
        {
            'a',
            'b',
            'c',
            'd',
            'e',
            'f',
            'g',
            'h'
        };

    }

    public override bool Equals(object obj)
    {
        return obj is TileCoordinates coord && coord.row == row && coord.column == column; 
    }

    public override string ToString()
    {
        return IsValid() ? $"{columnNames[column]}{row+1}" : "Not Valid Coordinates";
    }

    public bool IsValid() 
    {
        return (column >= 0 && row >= 0);
    }
}