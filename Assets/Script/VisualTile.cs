using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class VisualTile : MonoBehaviour, IPointerDownHandler
{
    public static Action<Tile> onTileSelected;

    [SerializeField]
    private Color darkColor;
    [SerializeField]
    private Color lightColor;
    [SerializeField]
    private Renderer tileRenderer;

    private Color actualColor;
    private Tile tile;

    public void SetDarkColor()
    {
        tileRenderer.material.color = darkColor;
        actualColor = darkColor;
    }

    public void SetLightColor()
    {
        tileRenderer.material.color = lightColor;
        actualColor = lightColor;
    }

    public void Paint() 
    {
        tileRenderer.material.color = actualColor;
    }

    public void Paint(Color color) 
    {
        tileRenderer.material.color = color;
    }

    public void SetTile(Tile tile) 
    {
        this.tile = tile;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onTileSelected?.Invoke(tile);
    }
}

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
            OccupiedBy = this.OccupiedBy,
            visualTile = null
        };
    }
}

public struct TileCoordinates 
{
    public int row;
    public int column;

    public TileCoordinates(int row, int column) 
    {
        this.row = row;
        this.column = column;
    }
}
