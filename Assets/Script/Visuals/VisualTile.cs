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


