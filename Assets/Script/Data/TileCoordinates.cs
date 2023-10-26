public struct TileCoordinates
{
    public int row;
    public int column;

    public TileCoordinates(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj) && obj is TileCoordinates coord && coord.row == row && coord.column == column; 
    }
}