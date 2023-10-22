using System;

public class Tile : IEnvironmentable
{
    public VisualTile visualTile { get; private set; }

    public Piece OccupiedBy { get; private set; }
    public bool IsOccupied => OccupiedBy != null;

    public TileCoordinates TilePosition { get; set; }

    public Environment Environment { get; }

    public Tile(Environment env) 
    {
        Environment = env;
    }

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

    public IEnvironmentable Copy(Environment env)
    {
        return new Tile(env)
        {
            TilePosition = this.TilePosition,
            OccupiedBy = (IsOccupied) ? this.OccupiedBy.Copy(env) as Piece : null,
            visualTile = null
        };
    }
}
