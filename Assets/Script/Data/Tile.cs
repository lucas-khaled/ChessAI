using System;

public class Tile
{
    public VisualTile visualTile { get; private set; }

    public Piece OccupiedBy { get; private set; }
    public bool IsOccupied => OccupiedBy != null;

    public TileCoordinates TilePosition { get; set; }

    public void SetVisual(VisualTile visualTile)
    {
        this.visualTile = visualTile;
        visualTile.SetTile(this);
    }

    public void Occupy(Piece piece)
    {
        OccupiedBy = piece;
    }

    public void DeOccupy()
    {
        OccupiedBy = null;
    }

    public bool IsVirtual()
    {
        return visualTile == null;
    }

    public Tile Copy()
    {
        return new Tile()
        {
            TilePosition = this.TilePosition,
            OccupiedBy = this.OccupiedBy.Copy() as Piece,
            visualTile = null
        };
    }
}
