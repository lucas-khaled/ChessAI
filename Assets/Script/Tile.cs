using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class Tile : MonoBehaviour, IPointerDownHandler
{
    public static Action<Tile> onTileSelected;

    [SerializeField]
    private Color darkColor;
    [SerializeField]
    private Color lightColor;
    [SerializeField]
    private Renderer tileRenderer;

    public Piece OccupiedBy { get; private set; }
    public bool IsOccupied => OccupiedBy != null;
    public Color ActualColor { get; private set; }

    public TileCoordinates TilePosition { get; set; }

    public void Occupy(Piece piece)
    {
        OccupiedBy = piece;
    }

    public void DeOccupy()
    {
        OccupiedBy = null;
    }

    public void SetDarkColor()
    {
        tileRenderer.material.color = darkColor;
        ActualColor = darkColor;
    }

    public void SetLightColor()
    {
        tileRenderer.material.color = lightColor;
        ActualColor = lightColor;
    }

    public void Paint(Color color) 
    {
        tileRenderer.material.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onTileSelected?.Invoke(this);
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
