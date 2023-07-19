using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private Color darkColor;
    [SerializeField]
    private Color lightColor;
    [SerializeField]
    private Renderer tileRenderer;

    public Piece OccupiedBy { get; private set; }
    public bool IsOccupied => OccupiedBy != null;
    public Color ActualColor { get; private set; }

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
}
